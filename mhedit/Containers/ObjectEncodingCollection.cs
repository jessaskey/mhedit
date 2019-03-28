using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mhedit.Containers
{
    public class ObjectEncodingCollection
    {
        public ObjectEncodingCollection()
        {
            ObjectEncodings = new List<ObjectEncoding>();
        }
        public List<ObjectEncoding> ObjectEncodings { get; set; }

        public List<byte> GetAllBytes()
        {
            List<byte> bytes = new List<byte>();
            foreach(ObjectEncoding o in ObjectEncodings)
            {
                bytes.AddRange(o.Bytes);
            }
            return bytes;
        }

        public void Add(ObjectEncoding encoding)
        {
            ObjectEncodings.Add(encoding);
        }

        public void Add(byte[] bytes, string comment)
        {
            ObjectEncoding encoding = new ObjectEncoding();
            encoding.Bytes.AddRange(bytes);
            encoding.Comment = comment;
            ObjectEncodings.Add(encoding);
        }

        public void Add(byte [] bytes, string comment, string group)
        {
            ObjectEncoding encoding = new ObjectEncoding();
            encoding.Bytes.AddRange(bytes);
            encoding.Comment = comment;
            encoding.Group = group;
            ObjectEncodings.Add(encoding);
        }

        public void Add(byte[] bytes, string comment, string group, string macro)
        {
            ObjectEncoding encoding = new ObjectEncoding();
            encoding.Bytes.AddRange(bytes);
            encoding.Comment = comment;
            encoding.Group = group;
            encoding.SourceMacro = macro;
            ObjectEncodings.Add(encoding);
        }

        public void Add(byte[] bytes)
        {
            ObjectEncoding encoding = new ObjectEncoding();
            encoding.Bytes.AddRange(bytes);
            encoding.Comment = "";
            ObjectEncodings.Add(encoding);
        }

        public void Add(byte b, string comment)
        {
            ObjectEncoding encoding = new ObjectEncoding();
            encoding.Bytes.Add(b);
            encoding.Comment = comment;
            ObjectEncodings.Add(encoding);
        }

        public void Add(byte b)
        {
            ObjectEncoding encoding = new ObjectEncoding();
            encoding.Bytes.Add(b);
            encoding.Comment = "";
            ObjectEncodings.Add(encoding);
        }
    }
}
