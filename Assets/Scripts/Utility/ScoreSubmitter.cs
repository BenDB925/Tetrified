
using UnityEngine;
using UnityEngine.Networking;

namespace Tetronimo.Scripts.Utility
{
    public class ScoreSubmitter
    {
        public void SubmitScore(string playerName, int score)
        {
            // Create a new WWWForm object to hold the data that we want to send
            WWWForm form = new WWWForm();

            // Add the player name and score to the form
            form.AddField("playerName", playerName);
            form.AddField("score", score);

            // Create a new UnityWebRequest to post the form data to your Squarespace website
            UnityWebRequest request = UnityWebRequest.Post("https://yourwebsite.squarespace.com/submit-score", form);

            // Send the request and wait for the response
            request.SendWebRequest();
        }
    }
}