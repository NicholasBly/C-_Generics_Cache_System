var cache = new SimpleCache<string>(60);

cache.Add("key1", "value1");
cache.Add("key2", "value2");

string value = cache.Get("key1");

cache.Remove("key2");

List<string> keys = cache.GetAllKeys();

cache.Clear();

//

var userCache = new SimpleCache<User>();
userCache.Add("NickBly", new User { Id = 319, Name = "Nick" });
