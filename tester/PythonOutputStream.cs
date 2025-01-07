using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tester
{
    public class PythonOutputStream : Stream
    {
        private readonly Action<string> _outputAction;
        private readonly MemoryStream _memoryStream;

        public PythonOutputStream(Action<string> outputAction)
        {
            _outputAction = outputAction;
            _memoryStream = new MemoryStream();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _memoryStream.Write(buffer, offset, count);
            _memoryStream.Position = 0;

            var reader = new StreamReader(_memoryStream, Encoding.Unicode);
            string text = reader.ReadToEnd();
            _memoryStream.SetLength(0);
            _outputAction(text);
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;
        public override long Position { get; set; }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }


}
