# M09-UF1-PR01 Práctica de criptografía

## ¿De qué va esto?

Este proyecto es una simulación donde trabajamos con **criptografía simétrica y asimétrica** para enviar un mensaje de forma segura, como si lo hiciéramos por Internet y no quisiéramos que nadie lo intercepte o lo modifique sin que nos enteremos.

Se divide en dos partes: la del **emisor** (quien manda el mensaje) y la del **receptor** (quien lo recibe). Cada uno tiene su papel para que todo se haga bien y seguro.

---

## ¿Qué hace exactamente?

### Parte del emisor:
1. **Firma el mensaje** con su clave privada, así el receptor sabrá que de verdad viene de él.
2. **Cifra el mensaje** usando AES (criptografía simétrica).
3. **Cifra la clave AES y el IV** con la clave pública del receptor usando RSA (criptografía asimétrica).

### Parte del receptor:
1. **Descifra la clave AES y el IV** con su clave privada.
2. **Descifra el mensaje** con AES.
3. **Comprueba la firma digital** del emisor con su clave pública, para asegurarse de que nadie ha tocado el mensaje.

---

## Archivos importantes

- `ClaveSimetrica.cs` → Clase que se encarga de generar claves AES y cifrar/descifrar mensajes simétricamente.
- `ClaveAsimetrica.cs` → Clase que genera claves RSA y permite firmar/verificar y cifrar/descifrar datos.
- `Program.cs` → Contiene la lógica principal donde se hace todo el proceso de envío y recepción.

---

## ¿Para qué sirve esto?

Este ejercicio ayuda a entender cómo funcionan los sistemas de seguridad en la vida real: desde enviar un mensaje en WhatsApp hasta proteger datos bancarios. Lo que vemos aquí es una versión simplificada pero totalmente funcional de cómo se manejan datos de forma segura.

---

## Requisitos técnicos

- .NET 6 o superior.
- Conocimientos básicos de C# y criptografía.
- Visual Studio o cualquier editor que soporte proyectos C#.

---

## Preguntas y respuestas sobre la práctica:
### Parte 1 - [DIB01] Explica el mecanismo de Registro / Login utilizado (máximo 5 líneas)

- Cuando el usuario se registra, escribe un nombre, y una contraseña. Para protegerla, se concatena una información llamado salt que es aleatorio antes de cifrarla en un algoritmo SHA512 que pedia el ejercicio. Ese resultado cifrado es lo que se guarda, no la contraseña real. Luego, al hacer login, se repite el mismo proceso: se coge la contraseña escrita, se añade el mismo salt, se cifra igual, y se compara con la versión guardada. Si coinciden deja entrar, si no, la contraseña es incorrecta.

### Parte 2 - Realiza una pequeña explicación de cada uno de los pasos que has hecho especificando el procedimiento que empleas en cada uno de ellos. 

 Por mi parte (el que manda el mensaje)
Le meto mi firma al texto
Lo primero que hago es estamparle una firma digital al mensaje, como si fuera mi firma personal. Así, cuando el otro lo reciba, podrá comprobar que nadie lo ha tocado y que lo mandé yo. Para eso uso mi clave privada y un resumen del mensaje con SHA512, que es como un resumen ultra blindado.

Encripto el mensaje con AES
Después, encripto el mensaje con AES, que es rápido y bastante seguro, pero necesito una clave y un IV (como una contraseña y un número inicial). Así, aunque alguien lo pille por el camino, no va a poder leer nada.

Encripto la clave del AES
Claro, ahora tengo que mandarle también esa clave y el IV al que recibe, pero no los puedo mandar tal cual. Por eso los encripto con su clave pública. Así solo él podrá desencriptarlos con su clave privada. Todo esto lo hago con RSA.

Por parte del que lo recibe
Desencripta la clave del AES
El receptor usa su clave privada para abrir la clave y el IV que le mandé. Así puede usar AES igual que yo y tiene lo necesario para desencriptar el mensaje.

Abre el mensaje
Con esa clave y ese IV, ya puede abrir el mensaje y ver lo que le mandé.

Comprueba que mi firma es válida
Por último, usa mi clave pública para verificar que la firma que puse encaja con el contenido. Si todo está bien, sabe que el mensaje es de verdad mío y que nadie ha metido mano por el camino.
