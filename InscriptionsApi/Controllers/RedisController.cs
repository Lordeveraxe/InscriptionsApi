using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Drawing;

namespace InscriptionsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly ConnectionMultiplexer _redisConnection;

        public RedisController(ConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
        }

        [HttpGet("{key}")]
        [AllowAnonymous]
        public IActionResult Get(string key)
        {
            var redisDatabase = _redisConnection.GetDatabase();

            // Obtiene el valor de la clave en Redis
            var value = redisDatabase.StringGet(key);

            if (value.HasValue)
            {
                return Ok(value.ToString());
            }

            return NotFound();
        }

        [HttpPut("{key}")]
        public IActionResult Put(string key, string value)
        {
            // Almacena el valor en Redis con la clave especificada
            var redisDatabase = _redisConnection.GetDatabase();
            redisDatabase.StringSet(key, value);

            return Ok();
        }


        /*[HttpPut("{key}")]
        public IActionResult PutWithTime(string key, string value, string time)
        {
            // Almacena el valor en Redis con la clave especificada
            var redisDatabase = _redisConnection.GetDatabase();

            TimeSpan tiempoExpiracion = TimeSpan.FromMinutes(Int32.Parse(time));
            redisDatabase.StringSet(key, value, tiempoExpiracion);

            return Ok();
        }*/
    }
}
