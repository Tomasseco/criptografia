using System;
using System.Text;
using System.Security.Cryptography;
using ClaveSimetricaClass;
using ClaveAsimetricaClass;

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
                        


                
                //LADO EMISOR

                //Firmar mensaje


                //Cifrar mensaje con la clave simétrica


                //Cifrar clave simétrica con la clave pública del receptor

                //LADO RECEPTOR

                //Descifrar clave simétrica

                 

                //Descifrar mensaje con la clave simétrica


                //Comprobar firma

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
