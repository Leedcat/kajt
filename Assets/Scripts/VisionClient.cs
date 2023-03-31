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
    private long movementTimeMilliseconds = 0;
    private ConcurrentQueue<int> scoreQueue = new ConcurrentQueue<int>();

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
        TcpListener server = new TcpListener(this.host, this.port);
        server.Start();

        while (true)
        {
            Debug.Log("Waiting for Vision Client to connect");
            using TcpClient client = server.AcceptTcpClient();
            Debug.Log("Vision Client connected");

            NetworkStream stream = client.GetStream();

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
            Debug.Log("Stopped Movement Stopwatch");
            this.movementStopwatch.Stop();
            this.movementTimeMilliseconds += this.movementStopwatch.ElapsedMilliseconds;
        }
        else if (classification == Classification.WALKING && !this.movementStopwatch.IsRunning)
        {
            Debug.Log("Started Movement Stopwatch");
            this.movementStopwatch.Start();
        }

        if (classification == Classification.ATTACKING)
        {
            if (this.attackStopwatch.IsRunning && this.attackStopwatch.ElapsedMilliseconds > 100)
            {
                Debug.Log("Stopped Attack Stopwatch");
                this.attackStopwatch.Stop();
                int score = this.CalculateScore();
                this.scoreQueue.Enqueue(score);
                this.movementTimeMilliseconds = 0;
            }
            else if (!this.attackStopwatch.IsRunning)
            {
                Debug.Log("Started Attack Stopwatch");
                this.attackStopwatch.Start();
            }
        }
    }

    private int CalculateScore()
    {
        // TODO: Switch this to getting from API
        long attackMinTimeMilliseconds = 1000;
        long attackDeltaTimeMilliseconds = this.attackStopwatch.ElapsedMilliseconds - attackMinTimeMilliseconds;

        return (int)(this.movementTimeMilliseconds / attackDeltaTimeMilliseconds);
    }
}
