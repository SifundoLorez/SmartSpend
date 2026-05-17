using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SmartSpend.Helpers
{
    public class EmailService
    {
        private static readonly string fromEmail = "22422492@dut4life.ac.za";
        private static readonly string fromName = "SmartSpend";
        private static readonly string appPassword = "Mafundwana012";

        public static async Task SendWelcomeEmailAsync(string toEmail, string firstName)
        {
            var subject = "Welcome to SmartSpend – Take Control of Your Finances";

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Welcome to SmartSpend</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', 'Inter', Arial, sans-serif;
                            line-height: 1.6;
                            color: #1e293b;
                        }}
                        .container {{
                            max-width: 580px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            text-align: center;
                            padding: 30px 0 20px;
                            border-bottom: 2px solid #e2e8f0;
                        }}
                        .logo {{
                            font-size: 28px;
                            font-weight: 800;
                            background: linear-gradient(135deg, #6366f1, #10b981);
                            -webkit-background-clip: text;
                            background-clip: text;
                            color: transparent;
                        }}
                        .content {{
                            padding: 30px 0;
                        }}
                        .btn {{
                            display: inline-block;
                            padding: 12px 28px;
                            background: linear-gradient(135deg, #6366f1, #8b5cf6);
                            color: #ffffff;
                            text-decoration: none;
                            border-radius: 8px;
                            font-weight: 600;
                            margin: 20px 0;
                        }}
                        .feature-list {{
                            background: #f8fafc;
                            padding: 20px;
                            border-radius: 12px;
                            margin: 20px 0;
                        }}
                        .feature-item {{
                            padding: 8px 0;
                            display: flex;
                            align-items: center;
                            gap: 10px;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 20px 0;
                            font-size: 12px;
                            color: #94a3b8;
                            border-top: 1px solid #e2e8f0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>SmartSpend</div>
                        </div>
                        
                        <div class='content'>
                            <h2 style='color: #0f172a; margin-bottom: 16px;'>Welcome to SmartSpend, {firstName}! 👋</h2>
                            
                            <p style='color: #334155;'>We're thrilled to have you on board. SmartSpend is your personal financial companion, designed to help you take control of your money with confidence.</p>
                            
                            <div class='feature-list'>
                                <div class='feature-item'>
                                    <span style='font-size: 18px;'>📊</span>
                                    <span><strong>Live Dashboard</strong> – See your finances at a glance</span>
                                </div>
                                <div class='feature-item'>
                                    <span style='font-size: 18px;'>🎯</span>
                                    <span><strong>Budget Control</strong> – Set limits and stay on track</span>
                                </div>
                                <div class='feature-item'>
                                    <span style='font-size: 18px;'>🧠</span>
                                    <span><strong>Smart Insights</strong> – AI-powered spending analysis</span>
                                </div>
                                <div class='feature-item'>
                                    <span style='font-size: 18px;'>📺</span>
                                    <span><strong>Subscription Tracker</strong> – Never miss a billing date</span>
                                </div>
                                <div class='feature-item'>
                                    <span style='font-size: 18px;'>🐷</span>
                                    <span><strong>Savings Goals</strong> – Watch your dreams become reality</span>
                                </div>
                            </div>
                            
                            <div style='text-align: center;'>
                                <a href='https://smartspend.co.za/Account/Login' class='btn'>Go to Dashboard →</a>
                            </div>
                            
                            <p style='color: #475569; margin-top: 20px;'>
                                <strong>Quick tips to get started:</strong>
                                <br>1️⃣ Add your first transaction – record an income or expense
                                <br>2️⃣ Set up your monthly budget – control where your money goes
                                <br>3️⃣ Create a savings goal – whether it's a car, holiday, or emergency fund
                            </p>
                            
                            <p style='color: #475569;'>Need help? Check out our <a href='https://smartspend.co.za/Help' style='color: #6366f1;'>Help Center</a> or reply to this email – we'd love to hear from you!</p>
                        </div>
                        
                        <div class='footer'>
                            <p>&copy; @DateTime.Now.Year SmartSpend · Built for South Africans 🇿🇦</p>
                            <p style='margin-top: 8px;'>
                                <a href='https://smartspend.co.za' style='color: #94a3b8; text-decoration: none;'>Website</a> | 
                                <a href='https://smartspend.co.za/Privacy' style='color: #94a3b8; text-decoration: none;'>Privacy Policy</a> | 
                                <a href='https://smartspend.co.za/Terms' style='color: #94a3b8; text-decoration: none;'>Terms of Service</a>
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            using (var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Timeout = 30000
            })
            using (var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            })
            {
                message.To.Add(toEmail);
                message.ReplyToList.Add(new MailAddress(fromEmail, fromName));

                await smtp.SendMailAsync(message);
            }
        }

        public static async Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink)
        {
            var subject = "Reset Your SmartSpend Password";

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Password Reset Request</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', 'Inter', Arial, sans-serif;
                            line-height: 1.6;
                            color: #1e293b;
                        }}
                        .container {{
                            max-width: 580px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            text-align: center;
                            padding: 30px 0 20px;
                            border-bottom: 2px solid #e2e8f0;
                        }}
                        .logo {{
                            font-size: 28px;
                            font-weight: 800;
                            background: linear-gradient(135deg, #6366f1, #10b981);
                            -webkit-background-clip: text;
                            background-clip: text;
                            color: transparent;
                        }}
                        .content {{
                            padding: 30px 0;
                        }}
                        .btn {{
                            display: inline-block;
                            padding: 12px 28px;
                            background: linear-gradient(135deg, #6366f1, #8b5cf6);
                            color: #ffffff;
                            text-decoration: none;
                            border-radius: 8px;
                            font-weight: 600;
                            margin: 20px 0;
                        }}
                        .warning {{
                            background: #fef3c7;
                            padding: 16px;
                            border-radius: 8px;
                            color: #92400e;
                            font-size: 14px;
                            margin: 20px 0;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 20px 0;
                            font-size: 12px;
                            color: #94a3b8;
                            border-top: 1px solid #e2e8f0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>SmartSpend</div>
                        </div>
                        
                        <div class='content'>
                            <h2 style='color: #0f172a; margin-bottom: 16px;'>Reset Your Password</h2>
                            
                            <p style='color: #334155;'>Hi <strong>{firstName}</strong>,</p>
                            
                            <p style='color: #334155;'>We received a request to reset the password for your SmartSpend account. Click the button below to create a new password:</p>
                            
                            <div style='text-align: center;'>
                                <a href='{resetLink}' class='btn'>Reset Password →</a>
                            </div>
                            
                            <div class='warning'>
                                <strong>⚠️ Security Notice</strong><br>
                                This link will expire in 1 hour. If you didn't request this, please ignore this email – your password will remain unchanged.
                            </div>
                            
                            <p style='color: #475569;'>For security reasons, never share this link with anyone. SmartSpend will never ask for your password via email.</p>
                        </div>
                        
                        <div class='footer'>
                            <p>&copy; @DateTime.Now.Year SmartSpend · Built for South Africans 🇿🇦</p>
                            <p style='margin-top: 8px;'>
                                <a href='https://smartspend.co.za' style='color: #94a3b8; text-decoration: none;'>Website</a> | 
                                <a href='https://smartspend.co.za/Support' style='color: #94a3b8; text-decoration: none;'>Support</a>
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            using (var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Timeout = 30000
            })
            using (var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = MailPriority.High
            })
            {
                message.To.Add(toEmail);
                await smtp.SendMailAsync(message);
            }
        }

        public static async Task SendSubscriptionReminderAsync(string toEmail, string firstName, string subscriptionName, decimal amount, DateTime billingDate)
        {
            var subject = $"Upcoming Payment: {subscriptionName}";

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Subscription Reminder</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', 'Inter', Arial, sans-serif;
                            line-height: 1.6;
                            color: #1e293b;
                        }}
                        .container {{
                            max-width: 580px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            text-align: center;
                            padding: 30px 0 20px;
                            border-bottom: 2px solid #e2e8f0;
                        }}
                        .logo {{
                            font-size: 28px;
                            font-weight: 800;
                            background: linear-gradient(135deg, #6366f1, #10b981);
                            -webkit-background-clip: text;
                            background-clip: text;
                            color: transparent;
                        }}
                        .content {{
                            padding: 30px 0;
                        }}
                        .subscription-details {{
                            background: #f8fafc;
                            padding: 20px;
                            border-radius: 12px;
                            margin: 20px 0;
                            text-align: center;
                        }}
                        .amount {{
                            font-size: 32px;
                            font-weight: 800;
                            color: #6366f1;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 20px 0;
                            font-size: 12px;
                            color: #94a3b8;
                            border-top: 1px solid #e2e8f0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>SmartSpend</div>
                        </div>
                        
                        <div class='content'>
                            <h2 style='color: #0f172a; margin-bottom: 16px;'>Subscription Reminder</h2>
                            
                            <p style='color: #334155;'>Hi <strong>{firstName}</strong>,</p>
                            
                            <p style='color: #334155;'>This is a friendly reminder about your upcoming subscription payment:</p>
                            
                            <div class='subscription-details'>
                                <div style='font-size: 18px; font-weight: 700; margin-bottom: 12px;'>{subscriptionName}</div>
                                <div class='amount'>R {amount:N2}</div>
                                <div style='margin-top: 12px; color: #64748b;'>
                                    <i class='fas fa-calendar-alt'></i> Due: {billingDate:dddd, MMMM d, yyyy}
                                </div>
                            </div>
                            
                            <p style='color: #475569;'>
                                <strong>Want to manage this subscription?</strong>
                                <br>→ Log in to SmartSpend to edit, pause, or cancel this subscription
                                <br>→ Review all your active subscriptions in one place
                            </p>
                            
                            <div style='text-align: center;'>
                                <a href='https://smartspend.co.za/Subscription/Index' class='btn' style='display: inline-block; padding: 10px 24px;'>Manage Subscriptions →</a>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>&copy; @DateTime.Now.Year SmartSpend · Built for South Africans 🇿🇦</p>
                            <p style='margin-top: 4px;'>You're receiving this because you have an active subscription in SmartSpend.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            using (var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Timeout = 30000
            })
            using (var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = MailPriority.Normal
            })
            {
                message.To.Add(toEmail);
                await smtp.SendMailAsync(message);
            }
        }

        public static async Task SendBudgetAlertAsync(string toEmail, string firstName, string categoryName, decimal spent, decimal limit, int percentage)
        {
            var subject = $"Budget Alert: {categoryName} - {percentage}% Used";

            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Budget Alert</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', 'Inter', Arial, sans-serif;
                            line-height: 1.6;
                            color: #1e293b;
                        }}
                        .container {{
                            max-width: 580px;
                            margin: 0 auto;
                            padding: 20px;
                        }}
                        .header {{
                            text-align: center;
                            padding: 30px 0 20px;
                            border-bottom: 2px solid #e2e8f0;
                        }}
                        .logo {{
                            font-size: 28px;
                            font-weight: 800;
                            background: linear-gradient(135deg, #6366f1, #10b981);
                            -webkit-background-clip: text;
                            background-clip: text;
                            color: transparent;
                        }}
                        .content {{
                            padding: 30px 0;
                        }}
                        .alert-box {{
                            background: {(percentage >= 100 ? "#fee2e2" : percentage >= 80 ? "#fef3c7" : "#e0f2fe")};
                            padding: 20px;
                            border-radius: 12px;
                            margin: 20px 0;
                            text-align: center;
                        }}
                        .percentage {{
                            font-size: 48px;
                            font-weight: 800;
                            color: {(percentage >= 100 ? "#dc2626" : percentage >= 80 ? "#d97706" : "#0284c7")};
                        }}
                        .footer {{
                            text-align: center;
                            padding: 20px 0;
                            font-size: 12px;
                            color: #94a3b8;
                            border-top: 1px solid #e2e8f0;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='logo'>SmartSpend</div>
                        </div>
                        
                        <div class='content'>
                            <h2 style='color: #0f172a; margin-bottom: 16px;'>Budget Alert</h2>
                            
                            <p style='color: #334155;'>Hi <strong>{firstName}</strong>,</p>
                            
                            <div class='alert-box'>
                                <div class='percentage'>{percentage}%</div>
                                <div style='font-size: 18px; font-weight: 600; margin: 12px 0;'>{categoryName} Budget</div>
                                <div style='color: #475569;'>Spent: R {spent:N2} of R {limit:N2}</div>
                            </div>
                            
                            {(percentage >= 100 ?
                                @"<p style='color: #dc2626;'><strong>⚠️ You've exceeded your budget!</strong> Consider reviewing your spending in this category.</p>" :
                                @"<p style='color: #d97706;'><strong>📊 You're approaching your budget limit.</strong> Keep an eye on your spending for the rest of the month.</p>")}
                            
                            <div style='text-align: center;'>
                                <a href='https://smartspend.co.za/Transaction/Index' class='btn' style='display: inline-block; padding: 10px 24px;'>View Transactions →</a>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>&copy; @DateTime.Now.Year SmartSpend · Built for South Africans 🇿🇦</p>
                            <p style='margin-top: 4px;'>Adjust your budget limits in the Budgets section of your dashboard.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            using (var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Timeout = 30000
            })
            using (var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                Priority = percentage >= 100 ? MailPriority.High : MailPriority.Normal
            })
            {
                message.To.Add(toEmail);
                await smtp.SendMailAsync(message);
            }
        }
    }
}