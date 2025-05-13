using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;
using System.Collections;

namespace SimuladorEnvioRecepcion
{
    class Program
    {   
        static string UserName;
        static string SecurePass;  
        static byte[] Salt;
        static ClaveAsimetrica Emisor = new ClaveAsimetrica();
        static ClaveAsimetrica Receptor = new ClaveAsimetrica();
        static ClaveSimetrica ClaveSimetricaEmisor = new ClaveSimetrica();
        static ClaveSimetrica ClaveSimetricaReceptor = new ClaveSimetrica();


        static string TextoAEnviar = "Me he dado cuenta que incluso las personas que dicen que todo está predestinado y que no podemos hacer nada para cambiar nuestro destino igual miran antes de cruzar la calle. Stephen Hawking.";
        
        static void Main(string[] args)
        {

            /****PARTE 1****/
            //Login / Registro
            Console.Clear();
            Console.WriteLine ("--------------------- LOGIN / REGISTRO ---------------------\n");
            Console.WriteLine ("¿Deseas registrarte? (S/N)");
            string registro = Console.ReadLine ();

            if (registro.ToLower() =="s")
            {
                //Realizar registro del cliente
                Console.WriteLine("----------------------- REGISTRO -----------------------");
                Registro();   
                Console.WriteLine("--------------------- FIN REGISTRO ---------------------\n");             

            }

            //Realizar login
            Console.WriteLine("------------------------ LOGIN -------------------------");
            bool login = Login();
            Console.WriteLine("---------------------- FIN LOGIN -----------------------");

            /***FIN PARTE 1***/

            if (login)
            {                  
                Console.WriteLine("\n--- Simulación de envío y recepción ---");

                //Este es el mensaje que vamos a enviar un mensaje al receptor
                
                byte[] TextoAEnviar_Bytes = Encoding.UTF8.GetBytes(TextoAEnviar);
                Console.WriteLine("Texto original: {0}", TextoAEnviar);

                //Firmar mensaje
                byte[] firmaDigital = Emisor.FirmarMensaje(TextoAEnviar_Bytes);
                Console.WriteLine("Mensaje firmado con la clave privada del emisor.");
                Console.WriteLine("Firma digital: {0}", BytesToStringHex(firmaDigital));

                //Cifrar mensaje con clave simétrica
                byte[] mensajeCifrado = ClaveSimetricaEmisor.CifrarMensaje(TextoAEnviar);
                Console.WriteLine("Mensaje cifrado con AES.");

                //Cifrar clave simétrica con la clave pública del receptor
                byte[] claveCifrada = Receptor.CifrarMensaje(ClaveSimetricaEmisor.Key, Receptor.PublicKey);
                byte[] ivCifrado = Receptor.CifrarMensaje(ClaveSimetricaEmisor.IV, Receptor.PublicKey);
                Console.WriteLine("Clave AES e IV cifrados con clave pública del receptor.");

                // ========== Enviamos los datos ==========
                // Cuando las claves y el mensaje se envian a traves de un canal inseguro, el receptor recibe los siguientes datos
                // Mensaje cifrado
                // Clave AES cifrada
                // IV cifrado
                // Firma digital
                // ========================================
                
                // Siguiendo el flujo del ejercicio vamos con el receptor
                // Receptor descifra la clave AES y IV
                
                byte[] claveDescifrada = Receptor.DescifrarMensaje(claveCifrada);
                byte[] ivDescifrado = Receptor.DescifrarMensaje(ivCifrado);
                ClaveSimetricaReceptor.Key = claveDescifrada;
                ClaveSimetricaReceptor.IV = ivDescifrado;
                Console.WriteLine("Clave AES y IV descifradas.");

                //Receptor descifra el mensaje con la clave AES
                string mensajeDescifrado = ClaveSimetricaReceptor.DescifrarMensaje(mensajeCifrado);
                Console.WriteLine("Mensaje descifrado: {0}", mensajeDescifrado);

                //Comprobar integridad del mensaje comparando hashes
                byte[] hashOriginal = SHA512.Create().ComputeHash(TextoAEnviar_Bytes);
                
                //Prueba para ver si realmente funciona la verificación de hashes
                //Modifico el mensaje descifrado para que no sea igual al original
                //mensajeDescifrado="Yo solo se que no se nada, y despues de la pedrá, menos todavia.";
                
                byte[] hashRecibido = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(mensajeDescifrado));
                bool hashesIguales = StructuralComparisons.StructuralEqualityComparer.Equals(hashOriginal, hashRecibido);
                Console.WriteLine("No hay diferencia en los dos hashes: {0}", hashesIguales);

              
                //Verificar firma con la clave pública del emisor
                bool firmaValida = Receptor.ComprobarFirma(firmaDigital, Encoding.UTF8.GetBytes(mensajeDescifrado), Emisor.PublicKey);
                Console.WriteLine("La firma es valida: {0}", firmaValida);

                //La hora de la verdad
                if (hashesIguales && firmaValida) {
                    Console.WriteLine("El mensaje es el mismo que el original.");
                }   else    {
                    Console.WriteLine("Quietor!, el mensaje no es el mismo que el original.");
                }
            
            }
        }

        public static void Registro()
        {
            Console.WriteLine("Indica tu nombre de usuario:");
            UserName = Console.ReadLine();

            Console.WriteLine("Indica tu password:");
            string passwordRegister = Console.ReadLine();

            // Generamos un salt aleatorio
            using (var rng = new RNGCryptoServiceProvider())
            {
                Salt = new byte[16];
                rng.GetBytes(Salt);
            }

            // Hash de la contraseña con SHA512 y el salt
            using (var sha512 = SHA512.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(passwordRegister);
                byte[] passwordConSalt = new byte[passwordBytes.Length + Salt.Length];

                Buffer.BlockCopy(passwordBytes, 0, passwordConSalt, 0, passwordBytes.Length);
                Buffer.BlockCopy(Salt, 0, passwordConSalt, passwordBytes.Length, Salt.Length);

                SecurePass = Convert.ToBase64String(sha512.ComputeHash(passwordConSalt));
            }

            Console.WriteLine("Se ha registrado correctamente.");
        }



        public static bool Login()
        {
            bool auxlogin = false;

            do
            {
                Console.WriteLine("Acceso a la aplicación");
                Console.WriteLine("Usuario: ");
                string userName = Console.ReadLine();

                Console.WriteLine("Password: ");
                string Password = Console.ReadLine();

                // Comprobar nombre de usuario
                if (userName != UserName)
                {
                    Console.WriteLine("Usuario incorrecto.");
                    continue;
                }

                // Recalcular el hash con el mismo salt
                using (var sha512 = SHA512.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(Password);
                    byte[] passwordConSalt = new byte[passwordBytes.Length + Salt.Length];

                    Buffer.BlockCopy(passwordBytes, 0, passwordConSalt, 0, passwordBytes.Length);
                    Buffer.BlockCopy(Salt, 0, passwordConSalt, passwordBytes.Length, Salt.Length);

                    string hashLogin = Convert.ToBase64String(sha512.ComputeHash(passwordConSalt));

                    // Comparar con hash original
                    if (hashLogin == SecurePass)
                    {
                        Console.WriteLine("Login correcto.");
                        auxlogin = true;
                    }
                    else
                    {
                        Console.WriteLine("Contraseña incorrecta.");
                    }
                }

            } while (!auxlogin);

            return auxlogin;
        }

        static string BytesToStringHex (byte[] result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (byte b in result)
                stringBuilder.AppendFormat("{0:x2}", b);

            return stringBuilder.ToString();
        }        
    }
}
