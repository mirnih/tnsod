namespace UserRequestsKafkaGenerator.Transport;

public sealed record KafkaEvent(int user_id, string endpoint);