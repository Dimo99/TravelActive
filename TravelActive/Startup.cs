using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelActive.Data;
using TravelActive.Infrastructure;
using TravelActive.Models.Entities;
using TravelActive.Services;

namespace TravelActive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseConfigurations(Configuration);
            services.AddIdentityConfigurations();
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            services.AddJwtTokenAuthorization(Configuration);
            services.AddCookieConfigurations();
            services.AddDomainServices();
            services.AddAutoMapper();
            services.Configure<AuthMessageSenderOptions>(Configuration.GetSection("SMTP"));
            services.AddMvcConfiguration();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider provider)
        {
            AddCycleStops(provider);
            app.UseStaticFiles();
            app.UseMvc();
        }

        private void AddCycleStops(IServiceProvider provider)
        {
            var context = provider.GetService<TravelActiveContext>();
            if (context.BicycleStops.Any())
            {
                return;
            }
            City city = new City()
            {
                Name = "Бургас"
            };
            context.Add(city);
            context.SaveChanges();
            Dictionary<string, LatLng> bicycleStops = new Dictionary<string, LatLng>()
            {
                {"Морска Гара",new LatLng(42.484761,27.482789) },
                { "Автогара Юг",new LatLng(42.490604,27.474128)},
                { "Казино",new LatLng(42.494932,27.482425)},
                {"Младежки културен център",new LatLng(42.496227,27.465877) },
                {"Пантеон",new LatLng(42.501500,27.482164) },
                {"Капани",new LatLng(42.506841,27.483571)},
                {"БСУ",new LatLng(42.503294,27.468482)},
                {"Бойчо Брънзов",new LatLng(42.514876,27.467595) },
                {"Мол Галерия (Славейков)",new LatLng(42.513546,27.453937) },
                {"\"Никола Петков\" (Изгрев)",new LatLng(42.519424,27.464046) },
                {"Парк Славейков",new LatLng(42.519989,27.451840) },
                {"С.К. \"Славейков\"",new LatLng(42.522353,27.447063) },
                {"Парк \"Изгрев\" (Велека)",new LatLng(42.523105,27.461618) },
                {"\"Двете брези\" (Зорница)" ,new LatLng(42.519392,27.467874)}
            };
            foreach (var stop in bicycleStops)
            {
                context.BicycleStops.Add(new BicycleStop()
                {
                    StopName = stop.Key,
                    CityId = city.Id,
                    Latitude = stop.Value.Latitude.ToString(),
                    Longitude = stop.Value.Longitude.ToString()
                });
            }

            context.SaveChanges();
        }

        private void TestDb(IServiceProvider provider)
        {
            var context = provider.GetService<TravelActiveContext>();
            var buses = context.Busses;
            foreach (var bus in buses)
            {
                var stopsOrdered = context.StopsOrdered.Include(s => s.BusStop).Where(x => x.BusId == bus.Id);
                List<LatLng> points = new List<LatLng>();
                foreach (var stopOrdered in stopsOrdered)
                {
                    points.Add(new LatLng(double.Parse(stopOrdered.BusStop.Latitude), double.Parse(stopOrdered.BusStop.Longitude)));
                }

                Console.WriteLine(Encode(points));
            }
        }
        private static string Encode(IEnumerable<LatLng> points)
        {
            var str = new StringBuilder();

            var encodeDiff = (Action<int>)(diff =>
            {
                int shifted = diff << 1;
                if (diff < 0)
                    shifted = ~shifted;
                int rem = shifted;
                while (rem >= 0x20)
                {
                    str.Append((char)((0x20 | (rem & 0x1f)) + 63));
                    rem >>= 5;
                }
                str.Append((char)(rem + 63));
            });

            int lastLat = 0;
            int lastLng = 0;
            foreach (var point in points)
            {
                int lat = (int)Math.Round(point.Latitude * 1E5);
                int lng = (int)Math.Round(point.Longitude * 1E5);
                encodeDiff(lat - lastLat);
                encodeDiff(lng - lastLng);
                lastLat = lat;
                lastLng = lng;
            }
            return str.ToString();
        }
        private void InitialDb(IServiceProvider provider)
        {
            var context = provider.GetService<TravelActiveContext>();
            if (context.Busses.Any())
            {
                return;
            }

            var busB1Meden = new Bus() { BusName = "Б1-Меден рудник" };
            var busB1Izgrev = new Bus() { BusName = "Б1-Изгрев" };
            var busB2Meden = new Bus() { BusName = "Б2-Меден рудник" };
            var busB2Izgrev = new Bus() { BusName = "Б2-Изгрев" };
            var bus15 = new Bus() { BusName = "15" };
            context.Busses.Add(busB1Meden);
            context.Busses.Add(busB1Izgrev);
            context.Busses.Add(busB2Meden);
            context.Busses.Add(busB2Izgrev);
            context.Busses.Add(bus15);
            Dictionary<string, LatLng> stops = new Dictionary<string, LatLng>()
        {
            {"Терминал Меден рудник",new LatLng(42.451615, 27.418778) },
            {"Бадемите [1]", new LatLng(42.455449, 27.418526) },
            {"Бадемите [2]", new LatLng(42.455397, 27.418306) },
            {"Резвая [1]", new LatLng(42.459098, 27.415704)},
            {"Резвая [2]", new LatLng(42.459063, 27.415526)},
            {"Захари Стоянов [1]", new LatLng(42.463218, 27.413655)},
            {"Захари Стоянов [2]", new LatLng(42.463280, 27.413479)},
            {"Езеро Вая [1]", new LatLng(42.472438, 27.429584) },
            {"Езеро Вая [2]", new LatLng(42.472794, 27.429548) },
            { "Спортна [1]",new LatLng(42.490874, 27.454128)},
            { "Спортна [2]",new LatLng(42.490935, 27.454168)},
            {"Терминал Юг",new LatLng(42.490531, 27.474512) },
            {"Тройката",new LatLng(42.497840, 27.468901) },
            {"Александровска [1]",new LatLng(42.502627, 27.468655) },
            {"Александровска [2]",new LatLng(42.502650, 27.468665) },
            {"Стефан Стамболов [1]",new LatLng(42.507712, 27.468465) },
            {"Стефан Стамболов [2]",new LatLng(42.507728, 27.468368) },
            {"Зорница [1]",new LatLng(42.514388, 27.463446) },
            {"Зорница [2]",new LatLng(42.514356, 27.463103) },
            {"Никола Петков [1]",new LatLng(42.520566, 27.464800) },
            {"Никола Петков [2]",new LatLng(42.520744, 27.464712) },
            {"Велека [1]",new LatLng(42.524874, 27.464061) },
            {"Велека [2]",new LatLng(42.524893, 27.464009) },
            {"Добри Чинтулов [1]",new LatLng(42.527304, 27.464507) },
            {"Добри Чинтулов [2]",new LatLng(42.527292, 27.464445) },
            {"Аква Калиде [1]",new LatLng(42.529975, 27.463051) },
            {"Аква Калиде [2]",new LatLng(42.530000, 27.462967) },
            {"Терминал Изгрев",new LatLng(42.529203, 27.458839) },
            {"Опера",new LatLng(42.494592, 27.470038) },
            {"Младост [1]",new LatLng(42.519912, 27.458724) },
            {"Младост [2]",new LatLng(42.519880, 27.458317) },
            {"Тракия- разделителна [1]",new LatLng(42.521220, 27.454248) },
            {"Тракия- разделителна [2]",new LatLng(42.521191, 27.454271) },
            {"Антон Страшимиров [1]",new LatLng(42.518347, 27.449436) },
            {"Антон Страшимиров [2]",new LatLng(42.518357, 27.449499) },
            {"Янко Комитов [1]",new LatLng(42.518481, 27.444311) },
            {"Янко Комитов [2]",new LatLng(42.518394, 27.444165) },
            {"Терминал Славейков",new LatLng(42.519698, 27.438370) },
            {"Автогара Юг",new LatLng(42.490250, 27.473698) },
            {"Булаир [1]",new LatLng(42.492076, 27.478651) },
            {"Булаир [2]",new LatLng(42.492105, 27.478631) },
            {"Ген. Гурко [1]",new LatLng(42.503570, 27.475069) },
            {"Ген. Гурко [2]",new LatLng(42.503504, 27.474847) },
            {"Сан Стефано [1]",new LatLng(42.503880, 27.474717) },
            {"Сан Стефано [2]",new LatLng(42.503847, 27.474514) },
            {"Дунав [1]",new LatLng(42.506781, 27.471640) },
            {"Дунав [2]",new LatLng(42.506784, 27.471401) },
            {"Демокрация [1]",new LatLng(42.509890, 27.468494) },
            {"Демокрация [2]",new LatLng(42.509894, 27.468265) },
            {"Изгрев, бл. 3",new LatLng(42.525161, 27.454216) },
            {"Транспортна [1]",new LatLng(42.529222, 27.457778) },
            {"Транспортна [2]",new LatLng(42.529364, 27.457711) },
            {"Гробищен парк [1]",new LatLng(42.531238, 27.464396) },
            {"Гробищен парк [2]",new LatLng(42.531399, 27.464294) },
            {"Летище Бургас",new LatLng(42.564910, 27.516607) },
            {"Сарафово, Октомври",new LatLng(42.561951, 27.520317) },
            {"Сарафово, Драва",new LatLng(42.559065, 27.518771) },
            {"Сарафово, Брацигово",new LatLng(42.561022, 27.522365) },
            {"Сарафово, Ради Николов",new LatLng(42.561022, 27.522365) },
            {"Р. Летище Бургас",new LatLng(42.564592, 27.518624) },
            {"Атанасовско езеро",new LatLng(42.544832, 27.473335) },
            {"Славейков, бл. 55",new LatLng(42.525125, 27.453712) },
            {"Тракия",new LatLng(42.522543, 27.456067) },

        };
            Dictionary<string, BusStop> busStops = new Dictionary<string, BusStop>();
            foreach (var item in stops)
            {
                BusStop busStop = new BusStop()
                {
                    Latitude = item.Value.Latitude.ToString(),
                    Longitude = item.Value.Longitude.ToString(),
                    StopName = item.Key
                };
                busStops.Add(item.Key, busStop);
                context.BusStops.Add(busStop);
            }
            context.SaveChanges();
            var busB1MedenStops = new List<string>() { "Терминал Меден рудник", "Бадемите [1]", "Резвая [1]", "Захари Стоянов [1]", "Езеро Вая [1]", "Спортна [1]", "Терминал Юг", "Тройката", "Александровска [1]", "Стефан Стамболов [1]", "Зорница [1]", "Никола Петков [1]", "Велека [1]", "Добри Чинтулов [1]", "Аква Калиде [1]", "Терминал Изгрев" };
            var busB1IzgrevStops = new List<string>() { "Терминал Изгрев", "Аква Калиде [2]", "Добри Чинтулов [2]", "Велека [2]", "Никола Петков [2]", "Зорница [2]", "Стефан Стамболов [2]", "Александровска [2]", "Опера", "Терминал Юг", "Спортна [2]", "Езеро Вая [2]", "Захари Стоянов [2]", "Резвая [2]", "Бадемите [2]", "Терминал Меден рудник" };
            var busB2MedenStops = new List<string>() { "Терминал Меден рудник", "Бадемите [1]", "Резвая [1]", "Захари Стоянов [1]", "Езеро Вая [1]", "Спортна [1]", "Терминал Юг", "Тройката", "Александровска [1]", "Стефан Стамболов [1]", "Зорница [1]", "Младост [1]", "Тракия- разделителна [1]", "Антон Страшимиров [1]", "Янко Комитов [1]", "Терминал Славейков" };
            var busB2IzgrevStops = new List<string>() { "Терминал Славейков", "Янко Комитов [2]", "Антон Страшимиров [2]", "Тракия- разделителна [2]", "Младост [2]", "Зорница [2]", "Стефан Стамболов [2]", "Александровска [2]", "Опера", "Терминал Юг", "Спортна [2]", "Езеро Вая [2]", "Захари Стоянов [2]", "Резвая [2]", "Бадемите [2]", "Терминал Меден рудник" };
            var bus15Stops = new List<string>() { "Автогара Юг", "Булаир [1]", "Ген. Гурко [1]", "Сан Стефано [1]", "Дунав [1]", "Демокрация [1]", "Зорница [1]", "Младост [1]", "Изгрев, бл. 3", "Транспортна [1]", "Гробищен парк [1]", "Летище Бургас", "Сарафово, Октомври", "Сарафово, Драва", "Сарафово, Брацигово", "Сарафово, Ради Николов", "Р. Летище Бургас", "Атанасовско езеро", "Гробищен парк [2]", "Транспортна [2]", "Славейков, бл. 55", "Тракия", "Младост [2]", "Зорница [2]", "Демокрация [2]", "Дунав [2]", "Сан Стефано [2]", "Ген. Гурко [2]", "Булаир [2]", "Автогара Юг" };
            AddBusStopsToBus(context, busB1Meden, busStops, busB1MedenStops);
            AddBusStopsToBus(context, busB1Izgrev, busStops, busB1IzgrevStops);
            AddBusStopsToBus(context, busB2Meden, busStops, busB2MedenStops);
            AddBusStopsToBus(context, busB2Izgrev, busStops, busB2IzgrevStops);
            AddBusStopsToBus(context, bus15, busStops, bus15Stops);
        }

        private static void AddBusStopsToBus(TravelActiveContext context, Bus bus, Dictionary<string, BusStop> busStops, List<string> busStopsStrings)
        {
            for (int i = 0; i < busStopsStrings.Count; i++)
            {
                var s = busStopsStrings[i];
                StopOrdered stopOrdered = new StopOrdered();
                stopOrdered.BusId = bus.Id;
                stopOrdered.BusStopId = busStops[s].Id;
                context.StopsOrdered.Add(stopOrdered);

                context.SaveChanges();

            }
            for (int i = 0; i < busStopsStrings.Count - 1; i++)
            {
                var s = busStopsStrings[i];
                for (int j = i + 1; j < busStopsStrings.Count; j++)
                {
                    var ds = busStopsStrings[j];
                    StopAccessibility stopAccessibility = new StopAccessibility();
                    stopAccessibility.BusId = bus.Id;
                    stopAccessibility.InitialStopId = busStops[s].Id;
                    stopAccessibility.DestStopId = busStops[ds].Id;
                    context.StopsAccessibility.Add(stopAccessibility);
                }
            }
            context.SaveChanges();


        }
    }
}
