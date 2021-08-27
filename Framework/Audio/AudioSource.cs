namespace Foster.Framework
{
    /// <summary>
    /// The Audio Source plays Audio Buffer
    /// </summary>
    public class AudioSource
    {
        public abstract class Platform
        {
            protected internal abstract void SetLooping(bool loop);
            protected internal abstract bool IsLooping();
            protected internal abstract void Play(AudioBuffer buffer);
            protected internal abstract void Dispose();
        }
        public readonly Platform Implementation;
        public AudioBuffer? Buffer;
        public bool Looping { 
            get => Implementation.IsLooping();
            set => Implementation.SetLooping(value);
        }

        public AudioSource(Audio audio)
        {
            Implementation = audio.CreateSource();
        }
        public void Play()
        {
            if (Buffer != null)
            {
                Implementation.Play(Buffer);
            }
        }
    }
}
