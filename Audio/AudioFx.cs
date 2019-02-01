using Microsoft.Xna.Framework;
using System.IO;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


namespace Splosion.Audio
{
    public class Cue
    {

        internal string Path;
        private readonly XAudio2 _device;
        private readonly byte[] _byteStream;
        private readonly List<SourceVoice> _voices;
        private SoundStream Stream
        {
            get
            {
                var m = new MemoryStream(_byteStream);
                // stream.
                return new SoundStream(m);
            }
        }

        public Cue(XAudio2 device, string path)
        {
            Path = path;
            _device = device;
            var s = Windows.ApplicationModel.Package.Current.InstalledLocation.OpenStreamForReadAsync(path);
            s.Wait();
            var stream = s.Result;
            _byteStream = ReadFully(stream);
            _voices = new List<SourceVoice>();
        }
        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public void Start()
        {

            SourceVoice s;
            if (_voices.Any(v => v.State.BuffersQueued <= 0))
            {
                s = _voices.First(v => v.State.BuffersQueued <= 0);
            }
            else
            {

                s = new SourceVoice(_device, Stream.Format, true);
                _voices.Add(s);
            }
            var b = new AudioBuffer
            {
                Stream = Stream.ToDataStream(),
                AudioBytes = (int)Stream.Length,
                Flags = BufferFlags.EndOfStream
            };
            s.SubmitSourceBuffer(b, Stream.DecodedPacketsInfo);
            s.Start();

        }
    }

    public class AudioFx
    {
        private MasteringVoice _masteringVoice;
        private readonly XAudio2 _xaudio2;
        private readonly List<Cue> _cues;


        public AudioFx()
        {
            _xaudio2 = new XAudio2();
            Task.Run(() =>
            {
                _xaudio2.StartEngine();
                _masteringVoice = new MasteringVoice(_xaudio2);
            });

            _cues = new List<Cue>();
        }

        public void Dispose()
        {
            _xaudio2.Dispose();
            _masteringVoice.Dispose();
        }
        public void SetVolume(float volume)
        {
            if (_masteringVoice != null)
            {
                _masteringVoice.SetVolume(volume);
            }
        }

        public Cue Play(string sound)
        {
            if (_cues.Any(p => p.Path == sound))
            {
                var c = _cues.First(p => p.Path == sound);
                c.Start();
                return c;
            }
            else
            {
                var c = new Cue(_xaudio2, sound);
                _cues.Add(c);
                c.Start();
                return c;
            }
        }

    }
}
