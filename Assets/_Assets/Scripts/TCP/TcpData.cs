using System;
using UnityEngine;

namespace _Assets.Scripts.TCP
{
    [CreateAssetMenu()]
    public class TcpData : ScriptableObject
    {
        public string serverIP;
        public int serverPort;
        public double sendInterval;
        public int buffer;
    }
}