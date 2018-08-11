
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace WindowsPhone_Tetris.Audio
{
    /// <summary>
    /// Helper class to manage audio tracks for music and sound
    /// </summary>
    public static class AudioHelper
    {
        private static Dictionary<String, Song> songs = new Dictionary<string,Song>();
        private static Dictionary<String, SoundEffect> soundEffects = new Dictionary<string,SoundEffect>();

        /// <summary>
        /// Adds a song to the Audio dictionary
        /// </summary>
        /// <param name="name">The name of the song for playback</param>
        /// <param name="song">The song instance to be played</param>
        public static void AddSong(String name, Song song)
        {
            songs.Add(name, song);
        }

        /// <summary>
        /// Adds a sound effect to the audio dictionary
        /// </summary>
        /// <param name="name">The name of the sound for playback</param>
        /// <param name="soundEffect">The sound effect instance to be played</param>
        public static void AddSoundEffect(String name, SoundEffect soundEffect)
        {
            soundEffects.Add(name, soundEffect);
        }

        /// <summary>
        /// Plays a song by the name identifier
        /// </summary>
        /// <param name="name">The name of the song to play</param>
        /// <param name="isLooping">Boolean value indicating whether or not the song should loop</param>
        public static void PlaySong(String name, bool isLooping)
        {
            MediaPlayer.IsRepeating = isLooping;
            MediaPlayer.Play(songs[name]);
        }

        /// <summary>
        /// Plays a sound effect by the name identifier
        /// </summary>
        /// <param name="name">The name of the sound to play</param>
        public static void PlaySound(String name)
        {
            soundEffects[name].Play();
        }

        /// <summary>
        /// Plays a sound effect by the name identifier
        /// </summary>
        /// <param name="name">The name of the sound to play</param>
        /// <param name="volume">Volume, ranging from 0.0f (silence) to 1.0f (full volume). 1.0f is full volume relative to SoundEffect.MasterVolume.</param>
        /// <param name="pitch">Pitch adjustment, ranging from -1.0f (down one octave) to 1.0f (up one octave). 0.0f is unity (normal) pitch.</param>
        /// <param name="pan">Panning, ranging from -1.0f (full left) to 1.0f (full right). 0.0f is centered.</param>
        public static void PlaySound(String name, float volume, float pitch, float pan)
        {
            soundEffects[name].Play(volume, pitch, pan);
        }

        /// <summary>
        /// Clears the currently playing sounds and songs as well as the collections. Once cleared, everything must be reloaded
        /// </summary>
        public static void Flush()
        {
            MediaPlayer.Stop();

            songs.Clear();
            soundEffects.Clear();
        }
    }
}
