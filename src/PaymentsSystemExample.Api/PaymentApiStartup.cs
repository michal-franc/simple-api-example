using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters;

namespace PaymentsSystemExample.Api
{
    public class PaymentApiStartup
    {
        public PaymentApiStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Singleton at the begginging as this is a service using in mem collection at the moment
            services.AddSingleton<IPaymentService, PaymentService>();
            // Will change to Transient when we have a proper DB connection
            //services.AddTransient<IPaymentService, PaymentService>();
            services.AddSingleton<IPaymentParser, PaymentParserJson>();
            services.AddTransient<IPaymentPersistenceService, PaymentPersistenceServiceDynamoDB>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //: TODO
                //app.UseHsts();
            }

            //: TODO
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
