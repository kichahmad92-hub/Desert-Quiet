using UnityEngine;

namespace DesertQuiet.Audio
{
    /// <summary>
    /// Small procedural sound library. Clips are synthesized at runtime so no external
    /// audio assets are required in the repository.
    /// </summary>
    public static class DesertAudio
    {
        private const int SampleRate = 22050;
        private static AudioClip wind;
        private static AudioClip gunshot;
        private static AudioClip impact;

        public static AudioClip Wind
        {
            get { if (wind == null) wind = CreateWind(); return wind; }
        }

        public static void PlayGunshot(Vector3 position)
        {
            if (gunshot == null) gunshot = CreateBurst("DQ Gunshot", 0.32f, 0.95f, 95f);
            AudioSource.PlayClipAtPoint(gunshot, position, 0.75f);
        }

        public static void PlayImpact(Vector3 position)
        {
            if (impact == null) impact = CreateBurst("DQ Impact", 0.16f, 0.55f, 180f);
            AudioSource.PlayClipAtPoint(impact, position, 0.6f);
        }

        private static AudioClip CreateWind()
        {
            const int length = 5 * SampleRate;
            float[] data = new float[length];
            uint seed = 2463534242;
            float low = 0f;
            for (int i = 0; i < length; i++)
            {
                seed ^= seed << 13;
                seed ^= seed >> 17;
                seed ^= seed << 5;
                float noise = ((seed & 0xffff) / 32767.5f) - 1f;
                low = Mathf.Lerp(low, noise, 0.018f);
                float swell = 0.65f + 0.35f * Mathf.Sin((float)i / SampleRate * 0.72f);
                data[i] = low * swell * 0.22f;
            }
            AudioClip clip = AudioClip.Create("DQ Desert Wind", length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        private static AudioClip CreateBurst(string name, float seconds, float volume, float frequency)
        {
            int length = Mathf.CeilToInt(seconds * SampleRate);
            float[] data = new float[length];
            uint seed = 123456789;
            for (int i = 0; i < length; i++)
            {
                float t = (float)i / SampleRate;
                float envelope = Mathf.Exp(-t * (name.Contains("Gunshot") ? 15f : 24f));
                seed = 1664525 * seed + 1013904223;
                float noise = ((seed >> 8) / 8388607.5f) - 1f;
                float tone = Mathf.Sin(2f * Mathf.PI * frequency * t);
                data[i] = (noise * 0.72f + tone * 0.28f) * envelope * volume;
            }
            AudioClip clip = AudioClip.Create(name, length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
