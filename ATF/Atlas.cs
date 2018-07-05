using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Atf
{
    public class Atlas {
        public int Width;
        public int Height;
        public bool Correct;
        private Bitmap _bitmap;
        private Dictionary<string, SubTexture> SubTextures = new Dictionary<string, SubTexture>();
        public List<string> Names() => SubTextures.Keys.ToList();

        public Atlas(Stream xmlStr, Bitmap bitmap) {
            using (var reader = new StreamReader(xmlStr)) {
                Init(reader.ReadToEnd(), bitmap);
            }
        }

        public Atlas(string xmlStr, Bitmap bitmap) {
            Init(xmlStr,bitmap);
        }

        private void Init(string xmlStr, Bitmap bitmap) {
            _bitmap = bitmap;
            var xml = XDocument.Parse(xmlStr);
            Width = Int32.Parse(xml.Root.Attribute("width")?.Value ?? "0");
            Height = Int32.Parse(xml.Root.Attribute("height")?.Value ?? "0");
            foreach (var sub in xml.Root.Elements("SubTexture"))
            {
                SubTextures[sub.Attribute("name")?.Value ?? "1"] = new SubTexture()
                {
                    x = Int32.Parse(sub.Attribute("x")?.Value ?? "0"),
                    y = Int32.Parse(sub.Attribute("y")?.Value ?? "0"),
                    width = Int32.Parse(sub.Attribute("width")?.Value ?? "0"),
                    height = Int32.Parse(sub.Attribute("height")?.Value ?? "0"),
                    frameX = Int32.Parse(sub.Attribute("frameX")?.Value ?? "0"),
                    frameY = Int32.Parse(sub.Attribute("frameY")?.Value ?? "0"),
                    frameWidth = Int32.Parse(sub.Attribute("frameWidth")?.Value ?? "0"),
                    frameHeight = Int32.Parse(sub.Attribute("frameHeight")?.Value ?? "0"),
                    rotated = Boolean.Parse(sub.Attribute("rotated")?.Value ?? "false")
                };
            }
            Correct = true;
            if (_bitmap != null && (_bitmap.Width < Width || _bitmap.Height < Height)) Correct = false;
            if (SubTextures.Any(x => x.Value.x < 0)) Correct = false;
            if (SubTextures.Any(x => x.Value.y < 0)) Correct = false;
            if (SubTextures.Any(x => x.Value.x + x.Value.width > Width)) Correct = false;
            if (SubTextures.Any(x => x.Value.y + x.Value.height > Height)) Correct = false;
        }

        public Bitmap GetTexture(string name) {
            if (!Correct || _bitmap==null) return new Bitmap(1,1);
            if (!SubTextures.TryGetValue(name,out SubTexture sub)) return new Bitmap(1,1);
            Bitmap res;
            if (sub.frameWidth>0 && sub.frameHeight>0)
                res = new Bitmap(sub.rotated? sub.frameHeight : sub.frameWidth,
                                 sub.rotated? sub.frameWidth : sub.frameHeight, PixelFormat.Format32bppArgb);
            else
                res = new Bitmap(sub.width, sub.height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(res)) {
                g.CompositingMode=CompositingMode.SourceCopy;
                g.DrawImage(_bitmap, sub.rotated? (sub.frameHeight==0?0:sub.frameHeight-sub.width+sub.frameY):-sub.frameX,
                                     sub.rotated?-sub.frameX:-sub.frameY, 
                                     new Rectangle(sub.x, sub.y, sub.width, sub.height),GraphicsUnit.Pixel);
            }
            if (sub.rotated) res.RotateFlip(RotateFlipType.Rotate270FlipNone);
            return res;
        }
    }

    public class SubTexture { //ints may be should be floats in general, but in our files there are no floats
        public int x;
        public int y;
        public int width;
        public int height;
        public int frameX;
        public int frameY;
        public int frameWidth;
        public int frameHeight;
        public bool rotated;
    }
}
