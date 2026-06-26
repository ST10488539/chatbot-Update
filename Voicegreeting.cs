using System;
using System.IO;       // FIXES: 'Path' and 'File' do not exist
using System.Media;    // FIXES: 'SoundPlayer' support

namespace WpfApp1
{
    public class Voicegreeting
    {
        // FIXES: '_audioFileName' does not exist
        private readonly string _audioFileName = "greeting.wav";

        public void PlayGreeting()
        {
            try
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _audioFileName);

                if (File.Exists(fullPath))
                {
                    SoundPlayer player = new SoundPlayer(fullPath);
                    player.Load();
                    player.Play();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Audio file missing at: {fullPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
            }
        }
    }
}
