namespace RateLimiter.Reader.Transport;

public sealed record UserRequest(int user_id, string endpoint);
