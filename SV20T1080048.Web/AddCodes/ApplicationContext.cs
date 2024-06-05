using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SV20T1080048.Web.AddCodes
{
    public class ApplicationContext
    {
        private static IHttpContextAccessor? _httpContextAccessor;
        private static IWebHostEnvironment? _hostEnvironment;
        private static IDistributedCache? _cache;

        public static void Configure(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment hostEnvironment, IDistributedCache cache)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// HttpContext
        /// </summary>
        public static HttpContext? HttpContext => _httpContextAccessor?.HttpContext;

        /// <summary>
        /// HostEnviroment
        /// </summary>
        public static IWebHostEnvironment? HostEnviroment => _hostEnvironment;

        /// <summary>
        /// Get the absolute path to the directory that contains the web-servable application content files.
        /// </summary>
        public static string WebRootPath => _hostEnvironment?.WebRootPath ?? string.Empty;

        /// <summary>
        /// Gets the absolute path to the directory that contains the application content files.
        /// </summary>
        public static string ContentRootPath => _hostEnvironment?.ContentRootPath ?? string.Empty;

        /// <summary>
        /// Ghi dữ liệu vào session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetSessionData(string key, object value)
        {
            try
            {
                string sValue = JsonConvert.SerializeObject(value);
                if (!string.IsNullOrEmpty(sValue))
                {
                    _httpContextAccessor?.HttpContext?.Session.SetString(key, sValue);
                }
            }
            catch
            {
                // Handle exception
            }
        }

        /// <summary>
        /// Đọc dữ liệu từ session
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T? GetSessionData<T>(string key) where T : class
        {
            try
            {
                string sValue = _httpContextAccessor?.HttpContext?.Session.GetString(key) ?? string.Empty;
                if (!string.IsNullOrEmpty(sValue))
                {
                    return JsonConvert.DeserializeObject<T>(sValue);
                }
            }
            catch
            {
                // Handle exception
            }
            return null;
        }

        /// <summary>
        /// Ghi dữ liệu vào Redis Cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <param name="slidingExpiration"></param>
        public static async Task SetCacheDataAsync(string key, object value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            try
            {
                string sValue = JsonConvert.SerializeObject(value);
                if (!string.IsNullOrEmpty(sValue))
                {
                    var options = new DistributedCacheEntryOptions();
                    if (absoluteExpiration.HasValue)
                        options.SetAbsoluteExpiration(absoluteExpiration.Value);
                    if (slidingExpiration.HasValue)
                        options.SetSlidingExpiration(slidingExpiration.Value);

                    await _cache.SetStringAsync(key, sValue, options);
                }
            }
            catch
            {
                // Handle exception
            }
        }

        /// <summary>
        /// Đọc dữ liệu từ Redis Cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<T?> GetCacheDataAsync<T>(string key) where T : class
        {
            try
            {
                string sValue = await _cache.GetStringAsync(key) ?? string.Empty;
                if (!string.IsNullOrEmpty(sValue))
                {
                    return JsonConvert.DeserializeObject<T>(sValue);
                }
            }
            catch
            {
                // Handle exception
            }
            return null;
        }
    }
}
