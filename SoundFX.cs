using System.Media;

namespace NEXUS;

public static class SoundFX
{
    private static bool muted;

    public static bool Muted
    {
        get => muted;
        set
        {
            muted = value;
            if (value)
            {
                try { ambientPlayer?.Stop(); } catch { }
            }
        }
    }

    // Se guardan las referencias: un SoundPlayer sin referencia puede ser recolectado por el GC y cortar el sonido.
    private static readonly Dictionary<string, SoundPlayer> PlayerCache = new();
    private static SoundPlayer? ambientPlayer;

    private static readonly byte[] LaserWav = BuildTone(880, 260, 0.08, 0.22, wave: 1);
    private static readonly byte[] HitWav = BuildTone(210, 95, 0.07, 0.25, wave: 2);
    private static readonly byte[] ExplosionWav = BuildNoise(0.22, 0.30);
    private static readonly byte[] PickupWav = BuildTone(520, 1080, 0.14, 0.24, wave: 0);
    private static readonly byte[] DashWav = BuildTone(320, 740, 0.13, 0.22, wave: 0);
    private static readonly byte[] PulseWav = BuildTone(640, 110, 0.28, 0.32, wave: 1);
    private static readonly byte[] LevelUpWav = BuildArpeggio(new[] { 523.25, 659.25, 783.99, 1046.50 }, 0.075, 0.25);

    private static readonly string[] AmbientFiles =
    {
        "eos_crystal_garden.wav",
        "nebula_submerged.wav",
        "sylvara_sanctuary.wav",
        "astralis_void.wav"
    };

    public static void PlayLaser() => PlayAsync("laser", LaserWav);
    public static void PlayHit() => PlayAsync("hit", HitWav);
    public static void PlayExplosion() => PlayAsync("explosion", ExplosionWav);
    public static void PlayPickup() => PlayAsync("pickup", PickupWav);
    public static void PlayDash() => PlayAsync("dash", DashWav);
    public static void PlayPulse() => PlayAsync("pulse", PulseWav);
    public static void PlayLevelUp() => PlayAsync("levelup", LevelUpWav);

    public static void PlayPlanetCue(int missionIndex)
    {
        if (Muted) return;
        try
        {
            int idx = Math.Clamp(missionIndex, 0, AmbientFiles.Length - 1);
            string path = Path.Combine(AppContext.BaseDirectory, "Assets", "Audio", AmbientFiles[idx]);
            if (File.Exists(path))
            {
                ambientPlayer?.Stop();
                ambientPlayer = new SoundPlayer(path);
                ambientPlayer.Play();
            }
        }
        catch
        {
            // Ignorar si no hay dispositivo de audio
        }
    }

    private static void PlayAsync(string key, byte[] wav)
    {
        if (Muted) return;
        try
        {
            if (!PlayerCache.TryGetValue(key, out var player))
            {
                player = new SoundPlayer(new MemoryStream(wav, writable: false));
                player.Load();
                PlayerCache[key] = player;
            }
            player.Play();
        }
        catch
        {
            // Ignorar si no hay dispositivo de audio disponible
        }
    }

    private static byte[] BuildTone(double fStart, double fEnd, double durationSec, double volume, int wave)
    {
        const int sampleRate = 22050;
        int samples = (int)(sampleRate * durationSec);
        short[] data = new short[samples];
        double phase = 0;
        for (int i = 0; i < samples; i++)
        {
            double t = (double)i / samples;
            double freq = fStart + (fEnd - fStart) * t;
            phase += 2.0 * Math.PI * freq / sampleRate;
            double env = (1.0 - t) * Math.Min(1.0, i / 120.0);
            double s = wave switch
            {
                1 => Math.Sign(Math.Sin(phase)) * 0.65,
                2 => (2.0 * (phase / (2.0 * Math.PI) - Math.Floor(0.5 + phase / (2.0 * Math.PI)))) * 0.75,
                _ => Math.Sin(phase)
            };
            data[i] = (short)(s * env * volume * short.MaxValue);
        }
        return WrapWav(data, sampleRate);
    }

    private static byte[] BuildNoise(double durationSec, double volume)
    {
        const int sampleRate = 22050;
        int samples = (int)(sampleRate * durationSec);
        short[] data = new short[samples];
        var rng = new Random(42);
        double lp = 0;
        for (int i = 0; i < samples; i++)
        {
            double t = (double)i / samples;
            double env = Math.Pow(1.0 - t, 1.6);
            double white = rng.NextDouble() * 2.0 - 1.0;
            lp = lp * 0.65 + white * 0.35;
            data[i] = (short)(lp * env * volume * short.MaxValue);
        }
        return WrapWav(data, sampleRate);
    }

    private static byte[] BuildArpeggio(double[] freqs, double noteSec, double volume)
    {
        const int sampleRate = 22050;
        int perNote = (int)(sampleRate * noteSec);
        short[] data = new short[perNote * freqs.Length];
        for (int n = 0; n < freqs.Length; n++)
        {
            double phase = 0;
            for (int i = 0; i < perNote; i++)
            {
                double t = (double)i / perNote;
                phase += 2.0 * Math.PI * freqs[n] / sampleRate;
                double env = (1.0 - t * 0.7) * Math.Min(1.0, i / 80.0);
                data[n * perNote + i] = (short)(Math.Sin(phase) * env * volume * short.MaxValue);
            }
        }
        return WrapWav(data, sampleRate);
    }

    private static byte[] WrapWav(short[] samples, int sampleRate)
    {
        int byteLen = samples.Length * 2;
        using var ms = new MemoryStream(44 + byteLen);
        using var bw = new BinaryWriter(ms);
        bw.Write("RIFF"u8.ToArray());
        bw.Write(36 + byteLen);
        bw.Write("WAVE"u8.ToArray());
        bw.Write("fmt "u8.ToArray());
        bw.Write(16);
        bw.Write((short)1);
        bw.Write((short)1);
        bw.Write(sampleRate);
        bw.Write(sampleRate * 2);
        bw.Write((short)2);
        bw.Write((short)16);
        bw.Write("data"u8.ToArray());
        bw.Write(byteLen);
        foreach (short s in samples)
            bw.Write(s);
        return ms.ToArray();
    }
}
