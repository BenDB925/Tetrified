using System.Collections.Generic;
using LootLocker.Requests;
using Tetrified.Scripts.Utility;
using UnityEditorInternal;
using UnityEngine;

namespace Tetronimo.Scripts.Utility
{
    public class OnlineScoreManager : Singleton<OnlineScoreManager>
    {
    	public struct ScoreData
        {
            public string Name;
            public int Score;
        }

        private List<ScoreData> _currentHighScores;

        private const string LootLockerLeaderBoardKey = "global";

        public void RetrieveScores(System.Action<List<ScoreData>> onScoresLoaded)
        {
            List<ScoreData> scoreList = new List<ScoreData>();

            LootLockerSDKManager.GetScoreList(LootLockerLeaderBoardKey, 10, 0, (response) =>
            {
                LootLockerGetScoreListResponse scores = response;

                foreach (LootLockerLeaderboardMember score in scores.items)
                {
                    ScoreData data = new ScoreData
                    {
                        Name = score.player.name,
                        Score = score.score
                    };

                    scoreList.Add(data);
                }

                onScoresLoaded(scoreList);
            });
        }

        public void InitSession()
        {
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (response.success)
                {
                }
                else
                {
                    Debug.LogError("can't connect to loot locker..");
                }
            });
        }

        public void SubmitScore(string playerName, int score)
        {
            LootLockerSDKManager.StartGuestSession(playerName , (response) =>
            {
                if (response.success)
                {
                    LootLockerSDKManager.SetPlayerName(playerName, (response) =>
                    {
                        if (response.success)
                        {
                            LootLockerSDKManager.SubmitScore(playerName, score, LootLockerLeaderBoardKey, (response) =>
                            {
                                if (response.statusCode == 200)
                                {
                                    Debug.Log("submitted score successfully!");
                                }
                                else
                                {
                                    Debug.Log("failed to submit score! Code: " + response.statusCode);
                                }
                            });
                        }
                    });
                }
                else
                {
                    Debug.LogError("can't connect to loot locker..");
                }
            });


        }
    }
}