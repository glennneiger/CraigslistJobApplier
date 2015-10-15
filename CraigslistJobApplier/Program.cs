﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using CraigslistJobApplier.Entities;

namespace CraigslistJobApplier
{
    class Program
    {
        static void Main(String[] args)
        {
            //TODO: check if user is actually online
            var options = new Options();
            CommandLine.Parser.Default.ParseArgumentsStrict(args, options);
            var validationErrors = InputValidator.GetValidationErrors(options);
            if (validationErrors.Any())
            {
                validationErrors.ForEach(e => Console.WriteLine(e));
                return;
            }

            var jobs = CraigslistJobExtractor.GetJobs(options.CraigslistUrl);
            Console.WriteLine("{0} emails produced", jobs.Count());

            var message = File.ReadAllText(options.MessageFile);
            var emails = EmailBuilder.GetEmails(jobs, message, options.Attachments);

            var emailer = new Emailer()
            {
                GmailAddress = options.GmailAddress,
                GmailPassword = options.GmailPassword,
                SentEmailsOutputFile = options.SentEmailsOutputFile,
                SecondsBetweenEmails = options.SecondsBetweenEmails
            };
            emailer.SendEmails(emails);
        }
    }
}
