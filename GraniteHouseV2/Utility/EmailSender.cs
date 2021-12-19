using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GraniteHouseV2.Utility
{
    public class EmailSender : IEmailSender
    {
        public IConfiguration _configuration { get; set; }

        public MailJetProps _mailJetProps { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return ExecuteV2(email, subject, htmlMessage);
        }

        //Mailjet implementation following MailJet documentation for Version 3.1
        public async Task Execute(string email, string subject, string body)
        {
            _mailJetProps = _configuration.GetSection("MailJet").Get<MailJetProps>();

            MailjetClient client = new MailjetClient(_mailJetProps.ApiKey, _mailJetProps.ApiSecret);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
                new JObject {
                    {
                       "From",
                       new JArray {
                        new JObject {
                            {"Email", "1_daveyjones@protonmail.com"},
                            {"Name", "Davey"}
                        }
                       }
                    }, {
                       "To",
                       new JArray {
                        new JObject {
                         {
                          "Email", email
                         }, {
                          "Name", "Davey Jones"
                         }
                        }
                       }
                    }, {
                       "Subject", subject
                    }, {
                       "HTMLPart", body
                    }
                }
             });
            MailjetResponse response = await client.PostAsync(request);
        }

        //MaiJet implementation following the MailJet.Api method
        public async Task ExecuteV2(string email, string subject, string body)
        {
            _mailJetProps = _configuration.GetSection("MailJet").Get<MailJetProps>();

            MailjetClient client = new MailjetClient(_mailJetProps.ApiKey, _mailJetProps.ApiSecret);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            };

            // construct your email with builder
            var _email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("1_daveyjones@protonmail.com"))
                .WithSubject(subject)
                .WithHtmlPart(body)
                .WithTo(new SendContact(email))
                .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(_email);
        }

        // MailJet implementation following MailJet documentation for Version 3
        public async Task ExecuteV3(string email, string subject, string body)
        {
            _mailJetProps = _configuration.GetSection("MailJet").Get<MailJetProps>();

            MailjetClient client = new MailjetClient(_mailJetProps.ApiKey, _mailJetProps.ApiSecret);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
                .Property(Send.FromEmail, "1_daveyjones@protonmail.com")
                .Property(Send.FromName, "Davey Jones")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, body)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", email}
                    }
                });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
