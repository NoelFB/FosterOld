using Foster.Framework;
using OpenAL;

namespace Foster.OpenAL
{
    internal class OpenAL_AudioSource : AudioSource.Platform
    {
        internal uint AL_Source;
        internal OpenAL_AudioSource()
        {
            AL10.alGenSources(1, out AL_Source);
        }
        protected override void SetLooping(bool loop)
        {
            AL10.alSourcei(AL_Source, AL10.AL_LOOPING, loop ? 1 : 0);
        }
        protected override bool IsLooping()
        {
            int v = 0;
            AL10.alGetSourcei(AL_Source, AL10.AL_LOOPING, out v);
            return v != 0;
        }
        protected override void Play(AudioBuffer buffer)
        {
            throw new System.NotImplementedException();
        }
        protected override void Dispose()
        {
            AL10.alDeleteSources(1, ref AL_Source);
        }
    }
}
