using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class API : MonoBehaviour
{
    private static readonly string URI = "https://localhost:2999/liveclientdata/activeplayer";
    public static API instance;
    private IEnumerator requestCoroutine;
    [field: SerializeField] private GameObject userInMatchUI;
    [field: SerializeField] public float attackSpeed { get; private set; }
    [field: SerializeField] public float attackTime { get; private set; }

    private void Start()
    {
        if (API.instance == null)
        {
            API.instance = this;
        }
        this.requestCoroutine = this.Request();
        StartCoroutine(this.requestCoroutine);
    }

    private void OnEnable()
    {
        if (this.requestCoroutine == null) return;

        StartCoroutine(this.requestCoroutine);
    }

    private void OnDisable()
    {
        if (this.requestCoroutine == null) return;

        StopCoroutine(this.requestCoroutine);
    }

    private IEnumerator Request()
    {
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(API.URI))
            {
                request.certificateHandler = new CustomCertificateHandler();
                yield return request.SendWebRequest();

                this.userInMatchUI.SetActive(request.result != UnityWebRequest.Result.Success);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("Request Error: " + request.error);
                    continue;
                }

                PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(request.downloadHandler.text);
                this.attackSpeed = playerInfo.championStats.attackSpeed;
                this.attackTime = 1 / this.attackSpeed;
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private class CustomCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    [Serializable]
    private struct PlayerInfo
    {
        public ChampionStats championStats;
    }

    [Serializable]
    private struct ChampionStats
    {
        public float attackSpeed;
    }
}
