namespace OrderManager.Infrastructure.Helper;

public static class MessageNames
{
    public class Exchange
    {
        public static string OrderExpired = "order-expired-exchange";
        public static string DeadLetter = "x-dead-letter-exchange";
    }

    public class Queue
    {
        public static string OrderExpiration = "order-expiration";
        public static string OrderExpired = "order-expired";
    }
}