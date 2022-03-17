using DataAccessLayer;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace MonthlyIncomeExpense
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MonthlyIncomeExpenseDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MssqlConnection")));

            services.AddLocalization(opt => { opt.ResourcesPath = "Resources"; });
            services.AddControllersWithViews().AddDataAnnotationsLocalization()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);

            

            services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("tr-TR"),
                    };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                {
                    //Write your code here
                    return new ProviderCultureResult("tr");
                }));
            });

            services.AddTransient<IRepository<IncomeExpense>, IncomeExpenseRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetRequiredService<MonthlyIncomeExpenseDbContext>())
                {
                    if (context.Database.EnsureCreated())
                    {
                        //will seed the db with existing 
                        SeedData(context);
                    }
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            //app.UseRequestLocalization(options.Value);

            IList<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("tr-TR")
                };

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            var requestProvider = new RouteDataRequestCultureProvider();
            requestLocalizationOptions.RequestCultureProviders.Insert(0, requestProvider);
            app.UseRequestLocalization(requestLocalizationOptions);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{culture=tr-TR}/{controller=IncomeExpense}/{action=Index}/{id?}");
            });
        }

        private void SeedData(MonthlyIncomeExpenseDbContext context)
        {
            context.IncomeExpenses.Add(new IncomeExpense()
            {
                Text = "Salary",
                Price = 1670,
                ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAIAAAAmKNuZAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAARKSURBVEhLrVXrT1tlGD/vOb0c6B16GZTCyjouAhJgQ6ATJk4xxg9m+s1EY/wXjH+CH0z84AcTjZfExGRojDh06lhcYjZYuEwGwwq0jHJpqbSlFE6v5+ava20dzGqyPXmTnvf2e3/P8/yep+TDqXdntm7ShFCPbJIskbd/ePOq9zuaMIW1RzBJEmma0AxRMIR5DINm6AJyWZMpGY6IsoiBD0wLG8fsv+FEWdAqdSdNrjZr5xOWznqDU82wAC1sP2iKwu+/GFzoqRt4teONZnObRqUDq3g6dic4Pbb05Ur47nGS5djBKbWyYsg5MlD/jMPYaNbYLBqb03R6qHGk297/0OyVg4N2WIZN8cnfArc2Yr6dg61gfPPe3sqdwLQsyyqFqnDuH1aeHaVjDX0N5zmeu+b7/tLCJ6OLn99Yv6ZkVJ21Z1hFxfGclI2dTCUyXDgResoxOOh8Pr9GKJISktd9P6aFNL7zi0Vj3K91r+2tkIdWBaEMrLHJ3OaP+fZSkWhyN3iwtbAzO7E6thZd3or7eSmb45dDzV0H2aPsEBT5vrJybsiUglZ21faCzub+ujfimdueVDEqZKPJ0t5gcoW50HJ40b+/Joh8/vpROK1ad0JnN6hNNE2n+VRaSP608u1I08u9dU9nhUyPfeDF5otWba1aweJwguf8e97PZj+4sT6Rj2MJDnOHwflK++u9jnM2nV1BK7jMoT/mvbp6+eOZ91ssHZBbn2NIz5oUjDLnBkWpGHVVpaVO70B5CZIAkAfYtdu6Xmi+aNFYCcllXKc2WLQ2FMDowqf1xsbQ4TYvCVeWv67VN2iUGjwPSYe4oJY1AvcoHGTJ0IpbG9dd1a1AwTTFJxBv5AHRRqfQqgx99UMIun/PF+ICKJjqSutg40gg7sduHqQIJ9MU6bb3gYVndyHq3xUkUaPS1urrzjkvLP05PxeY2ozdq9HX9dj73SeHQQd3UtlE4HBzMTjHH0sFkShpO77RZe97rur0/bTKFCFoYTvctiSL7oZhVIiRNS3szPASL0m5FgCClSoN3lAyyoyYxkqJHaFoXJ7e+FXPGhFvhtAZIR1NhmmaSfPp5d1FSM+qrTljdO+nY2khBaWhKUCYs1s3xb8bTKnIIEWXuRXqxyGEA9qOZ+KnqlsunHopyXMfTb93e3vqdmAKMq5QVDiNLnQqURIm/b8sheaL/arkLFrjZc8lZBOaOOtwo0VnhFQksfvN3S98EQ+8w1Z//XCYC/68OhbP7ON5S6WtxdJuYKvG/xjFYaCUMguDsqAGV3WLWXMC9ZDIHgTim6tRD5c5gMuxVNQb+R2Z6ag5m69JhBgo456v8lgw8s6Vtya848VMQ02lws5NclnJr+AMCsbd8Gyr9cmqSjP4Qi7zwZm5wGQsGcEBZOwoXHlDPaNZgDh0SGSC+8gy1vPvYfq/UIqWbzyCxPNCNitmUAkAKnkDD4p/UY9hSOJffKhZbABkU5oAAAAASUVORK5CYII="
            });
            context.IncomeExpenses.Add(new IncomeExpense()
            {
                Text = "Car",
                Price = -60,
                ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAIAAAAmKNuZAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAMDSURBVEhL7ZRJTBNRHMbnzZtOS1ugZZellFArBbS1EASCSmIxjZHE6MEtUeMSDxo9cPHkhYPxzoUYL94w4aDRg02IwYCoDUilZZEWS9laBNrafZkZX6cDFNuDCvHkL5OZzHxvvvff8gBWewXDAbZrAEVrauUA09zYK7sGVSXOve0egAGA7Z0dy3+7v+cf2DFMhuv32DHGOABCAU9AkhAHOMQxBskJ4jQdicUDwUicorlPaQCablRVptgxTHlR3v3Luhp5idXhisapmqp9xdLsD+ZvXn8wGI7OzDsNw5ZgJJqY1zSSdhAr0SZkhimUiu9dOnHzTNuX2cXHT18PGqcIgjiqVXb3vjQMm53r3uZD1Zr9FbZ5J7JmDXaYAoYpLZRwtUNp6prrLuqbhHzSYlvyegLhcGx23iURC/PEwlgo4lhYNQyZO9s1XVf1Z49rtAdkUrEApoVJcA8ctGkUpQUSCPHzJxtr5SUUxeRLxHm5oq5r+iVnEwMwPkkeVJQdqa/ynm5xuNyjk/Y+g/HjxFwUFXQT1o5JRFcgySYgRIl8dwf4fLJeUYaKiL50tNSxKxN4/aF3Y1/RXa2suH2uvU5Rdqf72aTDtZU4lywAgOQRro0fvf2D0/YV1McXb8cNIxYqZWek9vQN2BZXI7HYc4NxyDRbVVpQLBWjaLbg7GiaXvf4xqYdkMBb1QoUaau6+vPMgtsXTC5AvsMmW44oS6OUFUlyUFvem2yuDV80vr0fgu0sjtqChWPxdU8AJUiSxODoTKNKPmKyigSkLxB2rnmX1zwDn6YUFcXBUHTMYj+sqnwzYka9Ghq3+tHobHZ2e+4ghmULSP0x9YPrp4R8nnHS/ujJq4DHDwmkJAiEo50dDXcv6AR8HurAw55+55ovRFFJNW2MWfgQapXl+bmiibmVBZebThRmWxXyoLZGJhFnmazLi6tuJuXHzHYInKYBg9EApK7eglNRfX4ZY9YuwxFA4zgF8YxeCE5NG+AkmU6UXbB3duz0gZyGWxlPiD8F1a5eJfsJZ69UyS7Dtv0AAAAASUVORK5CYII="
            });
            context.IncomeExpenses.Add(new IncomeExpense()
            {
                Text = "Clothing",
                Price = -320,
                ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAbCAIAAADtdAg8AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAQGSURBVEhLrVVLbxtlFPW87PE4jt82TpzYtZO4akIbSmgkKkKlUlShigUSaxb8BX4CC/ZIrBDsKGUDggWLIiEEqkCAaBuaNKZOHDdO4uf4MeOZ8bw47jjO2AksCMef5fled+4592Hio3sfEA7ScWYYDiPiiRHv3H6DJKjB2hlgmHoykCFpkqZI6v8YNEVQZ6Jp4jOK/2jONA0n5YJYnNPjcBCDVYeDuvx2liD6c3xpkvl37jhlOgwcS4XmXpu/dfP8WyzNbtdzWISnfjZAW1YxmZpMrmVuBN1h+9vswJkin79X+P7i1Mpa+vXoRJzv1sSe0N85woAscuV8dOlCdFnRFZbhSJLsKK2gO9TtCZqhTrgmWzLPEPTq7KvPx1+8MvNKxPPc/dIvt//4GNZ1U7eMAMdkp33JmHfqq/XP2jK/WXnw4/ZdL+v/ZvOLp80dVVO+fvQ5yCZ8yabMZyOLpVbxzoNPCnzeMA3L0ChZ0yx3DjhmAiw4Jyep0qw/nQ4uXMvcdDNc1BOHXtnoEi/V3TQH94vNnbpYI4nxSA7nZlvhFU2mKVpQOk2pURXLoirUxHKjWwNxSesGuTCeQ56opqvlzp5haoOrNgzNET1V5qWapHYPhf0C/+Rx5WFNKOeqGwhcWdgvtYu6qdW71RAXFXsdkCVOuAYMyAKSLuHlK4mrsiZhtCQ+7IkiCCztDnkiiAD4QmUvO3nY3quKh8RpCXAUWYIA0/WD30utXRflCnERhMVJs6rea0i1/dbTttyqCmWGZBiKAQNdP4UpcBxZoCU38vWtx5X1Ir8NBZEiz+xOh7gw3Nwo38/VNhZjyy7ana9tQt/hRcCK7Ig5RB0+IsXKYmmXz+eqj3b4J3WxIvTalc7+w4PfmnI9FZjvp4vE5xs565YFyxzx7p03LVEx97GB6/O3koG0dQIwsYq34Ae6ECROohgQ4q3Kn5/++iHfrQ4DAleS/vRIdFjGnfClEr5zM/500p9J+udSgblzgYV0MIsBv3ABykIHHEZ1j/cTxGDoHUBTTDa8OBvICL0O6hEd0Vq3ABfR1NBFnLRzl9/+q7qB1BnsHXl3nCjPyAeXp1cvxC7tNQvfbn2JjBvLhaX45RcSq5Muv4tyQ9yuqo6lywhZVNWV2TWoczH+0qX4CkVCrGOgL1xNXV+ILE75Zl5OXYt645amdtjMmQ5Fl3uajCfV6KFsxzuV6UCDMQwdD7Imo9QG6zbY2idBoHo0Q4NAyImfd3/A1Do0REOquyi2o7R/KnyHNEJPHmwMtLIligVEAxdQDBj9JBkFlHLRLElSiirb4wCckigArAhKG8l80haARVSYqHTGbA1BQk47hkYG8xP4593+3wXx/t33LO3OCNiLeaf/Bud4RzY56m0JAAAAAElFTkSuQmCC"
            });
            context.IncomeExpenses.Add(new IncomeExpense()
            {
                Text = "Food",
                Price = -85,
                ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAbCAIAAADtdAg8AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAMOSURBVEhLrVVLTxNRFJ6ZO4+202lLmbbQh1AoJUCIiGgIJBgNoi40LlzoysSdf8KdK3f+Ad25M3HJwgAKaTSEKG95RCCxQOlD+phOXzOe0mlph6FA5Mtd3DP3nO885px78ddP3xIEjv03pILk8NrwV4/e4JdCJ8nONjuhSJeEi9DJR6suzkcnY7IskzSidBSO16vMuegQhVp7PPde3H748i7vttaJ8Rx0Mnaly3nn2VDPsN/maeSsRuz0+M6mYwxMz1Cn2W6en1gefzcZ3NhXDrSgTQeVgj4qrryk53S8x7o4vTr+fmrzx7YQF+BU0TsBdKtzTFVdkMy8qclrd7TYrM0WnYGmaDK0HdYbGUcLb2rkCERk0zlJkqoNwQXXwJKKVAbFUL5+b/egz2Qz0UzxNCNks5mc2+8020wUQ4IYDkZXv20sB9bFpFiyqqAmOoRQx4B35MmgkEgvzawtzfza+x3qHuxw+ZosDrOOZWgdBblDyM52h5jM7G+FZElJvBRdTe3Mdu7m/b7Q9sHkh0AhV2jva23pckO8ql8J7iHl62O9Nnej8qmMYzqgt3l4o5Wdn1rx9beNPR8Zfnxj4MFVYwOr2boWh8Xld1aiK6EmOgOnIykyshtz+ZuMFhZKDlDOToCikdGih3lR5CPUaBevFhyDNEmKqNOrFRxdRTV6x3Q4JufEHDQaazYICVGVxUmAArSLymlV7TDsMJzIZ3PXRnv55oYzL0FwGd2LKUIZVdHheCQYiwT/9o/2erpdUDjlQBMyFt2N7W6GVF5rbKDdFr6uiEIG1efCsHRKXA6sJaJJRS6jxgzKsT63Ffg0G48kTx1MGRPi6Z8TSwtfVgsFtY56Zgv5wsFOBAwgWUQSBEK5TD4RSwE78Kfi6T8be3OfF2fH51OH6ep2LE2FxtMDdtBuJp6zX+E5Kwv/OhUX9CxD6WigO9gJx/YPwauqt0tPz6kvGZDCBVVqF1AoxgZbAofW1tQ/4yUD54hEJE3CKm6o4oYkkSZXBVCdy1kIgX8Cn/74XXPCLwqoBmsx/APyrz4r0+/55gAAAABJRU5ErkJggg=="
            });
            context.IncomeExpenses.Add(new IncomeExpense()
            {
                Text = "Leisure",
                Price = -35,
                ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABsAAAAaCAIAAADJ6rCnAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAALNSURBVEhLrZRLSFRhFMf/Z+aO48xkTr4yXwRSI0klYvQiXEW5iHa2KCJaRO2KEFpI0CIIql0taxdREJS06B1YMkFkZhKSmpIaM9o4Oo5O87qnc2/XHPXONWJ+fIvLd+79f+fxvx/x0yMYug9CDmBCaaMNxLlcIJshnxWCLQ92t7bk4R9qsVYkuCt4yynec50b2rChGYoLkoclloqKh7eepV3XqPoghXox+QGcXjVLS0VOUfgzpvslNd54CF4f1LQRyo6lYjqO4YfoPIORx1R1gOtPQ/EaoexY95GRnEVkiFMRxMM0Nw41YUSyY62owUX1cJXD34buKxDp1SB+1rrC4YS8YjhLNG/JZJV8OAsQm4aaNF6TbsYnkJzWishEHF7SZKboKETTJZTtgM2he1AQ92a8kU7wSAf13UI6Co6DF1phriinFtTg8BuMv4KzCDUtmu5yGNEx/OhCIsSBt/T9iZ6voWjWRylWUht6gNHXWqUmEFylKKiEu5wa2+E7BpvTiGSZDGtOttmhpqCqxt5SeGaQ/e3oPMfBd1zdAqXQCJgoSvmpBOYnsKYGv8YQCxj7y5A67HmwKbKI1cwJmeWYjHDQz1X7MRfggbtIzOjdXQKtraXdl9F8g4obMPwIKb2POmaKqXkM3JEecW0rBbow1QfOUJSMYkGMv8RkN8Jf+P1FDN5bHLccZuIe+dzuhO8ENh3VfhLvZuSXGe4RJ071cf9tGn2B+aB2krgHC9PL4kcFcGntdHhQtQ+VcoO5jYh8kpylbx2Y+KT98iTti4k5jZhgoihb63ei7qShIhOXcWcimZLy557meAi9NynydTEbc8WSBmw/j/K98FRqozRFEoyO8thzfLxK0RFLRYEc8FRw0TbyHdcMZMrPHm2+oR5tRKtU/ReJuSv02pcFdBJhSMlYYX4rxf9DV9TvKzksV4tVYv8FiLlykyOwru43ZMVmb3tD3AQAAAAASUVORK5CYII="
            });
            context.IncomeExpenses.Add(new IncomeExpense()
            {
                Text = "Living",
                Price = -560,
                ImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAIAAAAmKNuZAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAMsSURBVEhLrVRLTxNRFO7MtDNlmL4f0s7wSIFCDY9aKQliRImRGClV1KVulFrRlH/g1oVbd66NWxcuNEYjUUyoiiJRMcSFgDSBIm3FptN2ZjzzsBaYVhL4cntz55xzv57z3XMvMhCbQVDN3sHzGi9DIt1X4iiCKLY9gOcFXyMpUu3f2Jc6y7ArOlQSQ5DW1fEfOo7T+JrI2AXmSIdxN3zV6ECO9saaiVHm4jHnjbPM0Q4TMApVSSvSga7tDWRslGllyMXVnN2oi4ZdQZ8Bw5AqhCp08v9DjZGQy99KTX9JP3yVnPyYctmIm+eYYJsBQ5BKOapmJ3Q1U+NhurvZ8OJDKrGeH+q1Qls9iW80OInYebq/04hWqGqnWfDWk9fDbqh0+nNmeY093Wvr8FCDAQsk9Dj+02HCr4XcfQeNSvhWbKdroUW9gOvt/K/lJBvqszstOIZorEbdUNCaZfnJ2RRYomH6cBul7CkDVhcYA9VhBTPoBeocaqGm5jKrG/nBgJV2EJJTA1MNgTIOYmE5u7TGdnpqfY21P5JsYr0guiXFHWZcyQ4+2ur14yN0l4d6OZdOZgonAhbYLHtLsJt0J4NWQRCezWwwNgLq6IEcy268QgcpuKyE1aR7/Sm9kmSP+811FhzspROEhTxsBt1AtwXej+fvU3ocazigl++MDMR/VXxRIM5MaT0ufYEXbl1uwrXoZo6DMLA3OPW5AreWEosCcptBazPjiXX2zoOlLMstrrLJVB5c0otSq5VIxezSv4vvFjbdNhx4b9//PjWXhioKReHuhPfrUvbeoxXYkC8Kl07VRYbdJIFmc1z8a0Zbntu2kxU9krdQEHJ5ns2LM7AUOXGRg0+Wh7UYAZnCiySvyrCz7zS8oIG+jYToSMgdHaHrnQQ0Cpw7VCAOJUodKnSgF9CNDbvGzriiI27GoZc7aTdQoQNgKKLFELjtMFeiUjWr0KnsL7NUT1RpFPkDRDbUYOF+O6nHZIuM2W+bb+YzIALHC/4WqsdrLHL805lUIsmW2OVG2UKnAPT+270y4BDLn2I5emuIQqemHXS/FF0a0u8fFKMaUGjUfRuc8AebRXiKsyHY8AAAAABJRU5ErkJggg=="
            });
            context.SaveChanges();
        }
    }
}
