using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using RestSharp;
using Tangle.Net.Entity;
using Tangle.Net.ProofOfWork;
using Tangle.Net.Repository;
using Tangle.Net.Repository.Client;
using Tangle.Net.Repository.Factory;
using Tangle.Net.Utils;
using Tangle.Net.Cryptography;
public class NewIOTA : MonoBehaviour
{

    string accountSeed = "";
    string nodeUri = "https://nodes.thetangle.org:443/";
    public InputField inp;
    public Text distance;
    int sendAmount = 1;
    public Button submit;
    void Start()
    {
        Button btn = submit.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    public void TaskOnClick()
    {
        if (true)
        {
            string message = "[From Leopard]: You traveled a distance of " + distance.text + " in 1 minute";
            string sendAddress = inp.text;
            SendMessage(nodeUri, sendAddress, message);

        }


    }

    void SendMessage(string nodeUri, string sendAddress, string message)
    {
        RestIotaClient iotaClient = new RestIotaClient(new RestClient(nodeUri));
        PoWService powService = new PoWService(new CpuPearlDiver()); // the examples will use the CPU to do PoW for a transaction.
        RestIotaRepository repository = new RestIotaRepository(iotaClient, powService);

        Bundle bundle = new Bundle();
        bundle.AddTransfer(new Transfer
        {
            Address = new Address(sendAddress),
            Message = TryteString.FromUtf8String(message),
            Tag = Tag.Empty,
            Timestamp = Timestamp.UnixSecondsTimestamp
        });

        bundle.Finalize();
        bundle.Sign();

        repository.SendTrytes(bundle.Transactions, 2, 14);

    }

    public void SendMessageWITOA(string accountSeed, string nodeUri, string sendAddress, int sendAmount, string message)
    {
        RestIotaClient iotaClient = new RestIotaClient(new RestClient(nodeUri));
        PoWService powService = new PoWService(new CpuPearlDiver()); // the examples will use the CPU to do PoW for a transaction.
        RestIotaRepository repository = new RestIotaRepository(iotaClient, powService);

        Seed sendSeed = new Seed(accountSeed);


        Bundle bundle = new Bundle();
        bundle.AddTransfer(
            new Transfer
            {
                Address = new Address(sendAddress),
                Tag = Tag.Empty,
                //ValueToTransfer = 0000000000000010,
                ValueToTransfer = sendAmount,
                Timestamp = Timestamp.UnixSecondsTimestamp
            });

        bundle.Finalize();
        bundle.Sign();
        repository.SendTransfer(sendSeed, bundle, SecurityLevel.Medium, 4, 4); // 4 for money // 2 for no money
    }
}