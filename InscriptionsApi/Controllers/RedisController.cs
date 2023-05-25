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

            return CreatedAtAction(nameof(Get), new { key = key }, value);
        }


        [HttpPut("{key},{value}")]
        public IActionResult PutWithTime(string key, string value, int time = 1)
        {
            // Almacena el valor en Redis con la clave especificada
            var redisDatabase = _redisConnection.GetDatabase();

            TimeSpan tiempoExpiracion = TimeSpan.FromMinutes(time);
            redisDatabase.StringSet(key, value, tiempoExpiracion);

            return CreatedAtAction(nameof(Get), new { key = key}, new { Value = value, Time = time });
        }
    }
}
