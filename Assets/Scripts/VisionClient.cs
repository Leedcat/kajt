using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections.Concurrent;

public class VisionClient : MonoBehaviour
{
    private int port = 9000;
    private IPAddress host = IPAddress.Parse("127.0.0.1");
    private Thread thread;
    private System.Diagnostics.Stopwatch attackStopwatch = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch movementStopwatch = new System.Diagnostics.Stopwatch();
    private ConcurrentQueue<int> scoreQueue = new ConcurrentQueue<int>();
    [field: SerializeField] private GameObject awaitingConnectionUI;
    [field: SerializeField] private GameObject connectedUI;
    [field: SerializeField] private float attackTime;
    [field: SerializeField] private float attackMinTime;
    [field: SerializeField] private float walkingTime;
    private TcpListener server;
    private bool isConnected = false;

    // Start is called before the first frame update
    void Start()
    {
        this.thread = new Thread(new ThreadStart(this.RecieveData));
        this.thread.IsBackground = true;
        this.thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        this.connectedUI.SetActive(this.isConnected);
        this.awaitingConnectionUI.SetActive(!this.isConnected);

        int score;
        if (!this.scoreQueue.TryDequeue(out score))
        {
            return;
        }

        // TODO: Show grade with something akin to ShowGrade.showgrade()
        Debug.Log("Score: " + score);
    }

    private void RecieveData()
    {
        byte[] buffer = new byte[256];
        this.server = new TcpListener(this.host, this.port);
        server.Start();

        while (true)
        {
            this.isConnected = false;
            Debug.Log("Waiting for Vision Client to connect");

            using TcpClient client = server.AcceptTcpClient();

            Debug.Log("Vision Client connected");
            this.isConnected = true;

            using NetworkStream stream = client.GetStream();

            int bytesRead = -1;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                Classification classification = (Classification)buffer[bytesRead - 1];

                this.UpdateTimes(classification);
            }

            Debug.Log("Vision Client disconnected");
        }
    }

    private void OnDisable()
    {
        if (this.server != null)
        {
            this.server.Stop();
        }
        this.thread.Abort();
    }

    private enum Classification
    {
        ATTACKING = 1,
        IDLING = 2,
        WALKING = 3,
    }

    private void UpdateTimes(Classification classification)
    {
        if (classification != Classification.WALKING && this.movementStopwatch.IsRunning)
        {
            this.movementStopwatch.Stop();
        }
        else if (classification == Classification.WALKING && !this.movementStopwatch.IsRunning)
        {
            this.movementStopwatch.Start();
        }

        if (classification == Classification.ATTACKING)
        {
            if (this.attackStopwatch.IsRunning && this.attackStopwatch.ElapsedMilliseconds > 100)
            {
                this.attackStopwatch.Stop();

                int score = this.CalculateScore();

                this.movementStopwatch.Reset();
                this.scoreQueue.Enqueue(score);
            }
            else if (!this.attackStopwatch.IsRunning)
            {
                this.attackStopwatch.Start();
            }
        }
    }

    private int CalculateScore()
    {
        this.walkingTime = this.movementStopwatch.ElapsedMilliseconds / 1000f;
        this.attackMinTime = API.instance.attackTime;
        this.attackTime = this.attackStopwatch.ElapsedMilliseconds / 1000f;

        return (int)((this.walkingTime / this.attackTime) * (this.attackMinTime / this.attackTime) * 100);
    }
}
