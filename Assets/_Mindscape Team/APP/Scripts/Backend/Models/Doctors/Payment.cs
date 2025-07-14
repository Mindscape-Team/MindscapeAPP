
public class Payment
{
    
}

[System.Serializable]
public class PaymentIntentResponse
{
    public string ClientSecret;
}

[System.Serializable]
public class PaymentRequest
{
    public int AppointmentId;
}

[System.Serializable]
public class ConfirmPaymentRequest
{
    public string PaymentIntentId;
}