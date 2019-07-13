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

public class ConnectIOTA : MonoBehaviour {

    public InputField accountSeed;
    public InputField nodeUri;   
    public InputField sendAddress;
    public InputField message;
    public InputField sendAmount;
    public Button submit;

        // Use this for initialization
    void Start () {
        Button btn = submit.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }


        // Update is called once per frame
    void Update () {
		
	}

    void TaskOnClick()
    {
        
        //if (int.Parse(sendAmount.text) > 0)
        //{
        //    SendMessageWITOA(accountSeed, nodeUri, sendAddress, sendAmount, message);

        //}
        //else
        //{

            SendMessage(nodeUri, sendAddress, message);
        //}
        
   
    }

    void SendMessage(InputField nodeUri, InputField sendAddress, InputField message)
    {

        RestIotaClient iotaClient = new RestIotaClient(new RestClient(nodeUri.text));
        PoWService powService = new PoWService(new CpuPearlDiver()); // the examples will use the CPU to do PoW for a transaction.
        RestIotaRepository repository = new RestIotaRepository(iotaClient, powService);

        Bundle bundle = new Bundle();
        bundle.AddTransfer(new Transfer
        {
            Address = new Address(sendAddress.text),
            Message = TryteString.FromUtf8String(message.text),
            Tag = Tag.Empty,
            Timestamp = Timestamp.UnixSecondsTimestamp
        });

        bundle.Finalize();
        bundle.Sign();

        repository.SendTrytes(bundle.Transactions, 27, 14);

    }

    //void SendMessageWITOA(InputField accountSeed, InputField nodeUri, InputField sendAddress, InputField sendAmount, InputField message)
    //{
    //    RestIotaClient iotaClient = new RestIotaClient(new RestClient(nodeUri.text));
    //    PoWService powService = new PoWService(new CpuPearlDiver()); // the examples will use the CPU to do PoW for a transaction.
    //    RestIotaRepository repository = new RestIotaRepository(iotaClient, powService);

    //    Seed sendSeed = new Seed(accountSeed.text);


    //    Bundle bundle = new Bundle();
    //    bundle.AddTransfer(
    //        new Transfer
    //        {
    //            Address = new Address(sendAddress.text),
    //            Tag = Tag.Empty,
    //            //ValueToTransfer = 0000000000000666,
    //            ValueToTransfer = Int64.Parse(sendAmount.text),
    //            Timestamp = Timestamp.UnixSecondsTimestamp
    //        });

    //    bundle.Finalize();
    //    bundle.Sign();

    //    repository.SendTransfer(sendSeed, bundle, SecurityLevel.Medium, 27, 4);

    //}


}







