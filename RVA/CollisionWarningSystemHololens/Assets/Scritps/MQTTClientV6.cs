using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using Newtonsoft.Json.Linq;


namespace M2MqttUnity.Examples
{
    public class MQTTClientV6 : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;

        public WarningSystemScriptV6 scriptReference;

        private List<string> eventMessages = new List<string>();

        public void SetBrokerAddress(string brokerAddress)
        {
            this.brokerAddress = brokerAddress;
        }

        public void SetBrokerPort(string brokerPort)
        {
            int.TryParse(brokerPort, out this.brokerPort);
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            Debug.Log("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("Connected to broker on " + brokerAddress + "\n");
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { "denm_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "vam_decoded" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            client.Subscribe(new string[] { "vam_set_heading" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "M2MQTT_Unity/test" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            Debug.LogError("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            Debug.LogError("CONNECTION LOST!");
        }

        protected override void Start()
        {
            scriptReference = GameObject.FindObjectOfType<WarningSystemScriptV6>();
            base.Start();
            Debug.Log("Ready.");
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);

            try {
                JObject json = JObject.Parse(msg);
                
                double latitude = 0.0; 
                double longitude = 0.0;
                float heading = 0f;

                if (topic == "denm_decoded")
                {
                    latitude = json["fields"]["denm"]["management"]["eventPosition"]["latitude"].Value<double>();
                    longitude =  json["fields"]["denm"]["management"]["eventPosition"]["longitude"].Value<double>();
                    //Debug.Log("[DENM] Latitude: " + latitude + ", Longitude: " + longitude);
                    
                    scriptReference.SetDENMCoordinates(latitude, longitude);
                    scriptReference.DisplayWarningSystem();
                }

                if (topic == "vam_decoded")
                {
                    latitude = json["vam"]["vamParameters"]["referencePosition"]["latitude"].Value<double>();
                    longitude =  json["vam"]["vamParameters"]["referencePosition"]["longitude"].Value<double>();

                    latitude = latitude / 10000000;
                    longitude = longitude / 10000000;
                    //Debug.Log("[VAM] Latitude: " + latitude + ", Longitude: " + longitude);

                    scriptReference.SetVAMCoordinates(latitude, longitude);
                }

                if (topic == "vam_set_heading")
                {
                    heading = json["heading"].Value<float>();
                    Debug.Log("[MQTT HEADING] Heading: " + heading);

                    scriptReference.SetHeading(heading);
                }
            } catch (Exception e) {
                Debug.Log("Failed to parse message on topic: " + topic + " msg: " + msg + ", Exception: " + e.Message);
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            //AddUiMessage("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }
    }
}
