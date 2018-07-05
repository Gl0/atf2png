using System;
using System.Drawing.Imaging;
using System.IO;
using Atf;

namespace atf2png
{
    class Program
    {
        static void Main(string[] args) {
            if (args.Length < 1 || args.Length > 2) {
                Console.WriteLine("\r\nUsage: atf2png test.atf [atlas.xml]\r\n");
                Console.WriteLine("Parameters:\r\ntest.atf      Path to .atf file to extract");
                Console.WriteLine(               "atlas.xml     Path to texture atlas .xml file (optional)");
                Console.WriteLine(               "              If atlas is not specified, program will look ");
                Console.WriteLine(               "              for .xml file with same name as atf one (test.xml in example)\r\n");
                Console.WriteLine(               "              If no texture atlas found - full image will be saved as test.png");
                Console.WriteLine(               "              otherwise all subtextures will be saved in the folder 'test'");
                return;
            }
            var atfname = args[0];
            if (!File.Exists(atfname)) {
                Console.WriteLine("File "+atfname+" not found!");
                return;
            }
            ATFReader atf;
            try { using (FileStream atFileStream = new FileStream(atfname, FileMode.Open, FileAccess.Read)) { atf = new ATFReader(atFileStream); } }
            catch(Exception e) { Console.WriteLine(e.Message);return;}
            string xmlFileName;
            var nameWithoutExtension = Path.Combine(Path.GetDirectoryName(atfname), Path.GetFileNameWithoutExtension(atfname));
            if (args.Length == 2) xmlFileName = args[1];
            else xmlFileName = nameWithoutExtension + ".xml";
            if (!File.Exists(xmlFileName)) {
                Console.WriteLine("Saving " + nameWithoutExtension + ".png");
                atf.Bitmap.Save(nameWithoutExtension + ".png", ImageFormat.Png); return;
            }
            var xml = File.ReadAllText(xmlFileName);
            Atlas atlas = new Atlas(xml, atf.Bitmap);
            if (!atlas.Correct) { Console.WriteLine("Incorrect atlas!"); return; }
            Directory.CreateDirectory(nameWithoutExtension);
            Console.WriteLine("Saving textures into " + nameWithoutExtension);
            atlas.Names().ForEach(x => atlas.GetTexture(x).Save(Path.Combine(nameWithoutExtension, x + ".png")));
        }
    }
}
