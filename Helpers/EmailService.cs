using System;
using System.Net;
using System.Net.Mail;

namespace SmartSpend.Helpers
{
    public static class EmailService
    {
        private const string FromEmail = "sfundob284@gmail.com";
        private const string AppPassword = "hyjfevzpniqlidfj";
        private const string AppUrl = "https://localhost:44366";

        public static bool SendWelcomeEmail(string toEmail, string firstName)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(FromEmail, "SmartSpend");
                mail.To.Add(new MailAddress(toEmail, firstName));
                mail.Subject = "Welcome to SmartSpend! 🎉";
                mail.IsBodyHtml = true;
                mail.Body = BuildWelcomeEmail(firstName);

                var smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, AppPassword);

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email error: " + ex.Message);
                return false;
            }
        }

        private static string BuildWelcomeEmail(string firstName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin:0;padding:0;background:#0f0f1a;
             font-family:Segoe UI,Arial,sans-serif;'>

  <table width='100%' cellpadding='0' cellspacing='0'
         style='background:#0f0f1a;padding:40px 20px;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0'
               style='max-width:600px;width:100%;'>

          <!-- Logo -->
          <tr>
            <td align='center' style='padding-bottom:28px;'>
              <div style='background:linear-gradient(135deg,#6366f1,#10b981);
                          border-radius:14px;padding:12px 24px;
                          display:inline-block;'>
                <span style='font-size:22px;font-weight:800;
                             color:#fff;letter-spacing:-0.5px;'>
                  💰 SmartSpend
                </span>
              </div>
            </td>
          </tr>

          <!-- Card -->
          <tr>
            <td style='background:#1a1a2e;
                       border:1px solid rgba(255,255,255,0.08);
                       border-radius:20px;overflow:hidden;'>

              <!-- Top bar -->
              <tr>
                <td style='background:linear-gradient(90deg,#6366f1,#8b5cf6,#10b981);
                           height:4px;font-size:0;line-height:0;'>&nbsp;</td>
              </tr>

              <!-- Body -->
              <tr>
                <td style='padding:40px;'>

                  <p style='font-size:52px;text-align:center;margin:0 0 20px;'>🎉</p>

                  <h1 style='color:#fff;font-size:28px;font-weight:800;
                             text-align:center;margin:0 0 14px;
                             letter-spacing:-0.5px;'>
                    Welcome, {firstName}!
                  </h1>

                  <p style='color:rgba(255,255,255,0.55);font-size:16px;
                            text-align:center;line-height:1.7;margin:0 0 32px;'>
                    Your SmartSpend account has been created successfully.<br/>
                    You're one step closer to financial freedom. 🚀
                  </p>

                  <!-- CTA -->
                  <table width='100%' cellpadding='0' cellspacing='0'>
                    <tr>
                      <td align='center' style='padding-bottom:36px;'>
                        <a href='{AppUrl}/Account/Login'
                           style='background:linear-gradient(135deg,#6366f1,#8b5cf6);
                                  color:#fff;text-decoration:none;
                                  padding:16px 44px;border-radius:12px;
                                  font-size:16px;font-weight:700;
                                  display:inline-block;
                                  box-shadow:0 4px 20px rgba(99,102,241,0.4);'>
                          Go to My Dashboard →
                        </a>
                      </td>
                    </tr>
                  </table>

                  <!-- Divider -->
                  <hr style='border:none;
                             border-top:1px solid rgba(255,255,255,0.07);
                             margin:0 0 32px;'/>

                  <!-- Features heading -->
                  <p style='color:rgba(255,255,255,0.45);font-size:12px;
                            text-transform:uppercase;letter-spacing:1.5px;
                            text-align:center;margin:0 0 18px;'>
                    Everything included in your account
                  </p>

                  <!-- Feature rows -->
                  {FeatureRow("📊", "Live Dashboard", "Real-time income, expenses and balance overview")}
                  {FeatureRow("🎯", "Budget Control", "Set monthly limits per category with warnings")}
                  {FeatureRow("🧠", "Smart Insights", "Automatic spending analysis and comparisons")}
                  {FeatureRow("🐷", "Savings Goals", "Track progress toward your financial targets")}
                  {FeatureRow("📺", "Subscription Tracker", "Manage Netflix, Spotify, DStv and more")}
                  {FeatureRow("💱", "Currency Converter", "Live ZAR to USD, EUR, GBP exchange rates")}
                  {FeatureRow("🔔", "Smart Notifications", "Budget alerts and subscription reminders")}
                  {FeatureRow("📄", "PDF Reports", "Export monthly financial reports anytime")}

                </td>
              </tr>

              <!-- Card footer -->
              <tr>
                <td style='background:rgba(255,255,255,0.03);
                           border-top:1px solid rgba(255,255,255,0.07);
                           padding:20px 40px;text-align:center;'>
                  <p style='color:rgba(255,255,255,0.3);font-size:13px;
                            margin:0 0 6px;'>
                    You received this because you registered on SmartSpend.
                  </p>
                  <p style='color:rgba(255,255,255,0.2);font-size:12px;margin:0;'>
                    © {DateTime.Now.Year} SmartSpend · Built for South Africa 🇿🇦
                  </p>
                </td>
              </tr>

            </td>
          </tr>

          <tr><td style='height:32px;'></td></tr>

        </table>
      </td>
    </tr>
  </table>

</body>
</html>";
        }

        private static string FeatureRow(string icon, string title, string desc)
        {
            return $@"
            <table width='100%' cellpadding='0' cellspacing='0'
                   style='background:rgba(255,255,255,0.04);
                          border:1px solid rgba(255,255,255,0.07);
                          border-radius:12px;margin-bottom:10px;'>
              <tr>
                <td style='padding:14px 16px;'>
                  <table cellpadding='0' cellspacing='0'>
                    <tr>
                      <td style='font-size:22px;padding-right:14px;
                                 vertical-align:middle;'>{icon}</td>
                      <td style='vertical-align:middle;'>
                        <div style='color:#fff;font-size:14px;
                                   font-weight:700;margin-bottom:3px;'>
                          {title}
                        </div>
                        <div style='color:rgba(255,255,255,0.45);
                                   font-size:13px;'>
                          {desc}
                        </div>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>";
        }

        public static bool SendPasswordResetEmail(string toEmail,
                                           string firstName,
                                           string resetLink)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(FromEmail, "SmartSpend");
                mail.To.Add(new MailAddress(toEmail, firstName));
                mail.Subject = "Reset Your SmartSpend Password 🔐";
                mail.IsBodyHtml = true;
                mail.Body = BuildResetEmail(firstName, resetLink);

                var smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(FromEmail, AppPassword);

                smtp.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Reset email error: " + ex.Message);
                return false;
            }
        }

        private static string BuildResetEmail(string firstName, string resetLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin:0;padding:0;background:#0f0f1a;
             font-family:Segoe UI,Arial,sans-serif;'>

  <table width='100%' cellpadding='0' cellspacing='0'
         style='background:#0f0f1a;padding:40px 20px;'>
    <tr>
      <td align='center'>
        <table width='600' cellpadding='0' cellspacing='0'
               style='max-width:600px;width:100%;'>

          <!-- Logo -->
          <tr>
            <td align='center' style='padding-bottom:28px;'>
              <div style='background:linear-gradient(135deg,#6366f1,#10b981);
                          border-radius:14px;padding:12px 24px;
                          display:inline-block;'>
                <span style='font-size:22px;font-weight:800;
                             color:#fff;letter-spacing:-0.5px;'>
                  💰 SmartSpend
                </span>
              </div>
            </td>
          </tr>

          <!-- Card -->
          <tr>
            <td style='background:#1a1a2e;
                       border:1px solid rgba(255,255,255,0.08);
                       border-radius:20px;overflow:hidden;'>

              <!-- Top bar -->
              <tr>
                <td style='background:linear-gradient(90deg,#6366f1,#8b5cf6,#f87171);
                           height:4px;font-size:0;line-height:0;'>&nbsp;</td>
              </tr>

              <!-- Body -->
              <tr>
                <td style='padding:40px;'>

                  <p style='font-size:48px;text-align:center;margin:0 0 20px;'>🔐</p>

                  <h1 style='color:#fff;font-size:26px;font-weight:800;
                             text-align:center;margin:0 0 14px;
                             letter-spacing:-0.5px;'>
                    Password Reset Request
                  </h1>

                  <p style='color:rgba(255,255,255,0.55);font-size:15px;
                            text-align:center;line-height:1.7;margin:0 0 28px;'>
                    Hi {firstName}, we received a request to reset your password.<br/>
                    Click the button below to create a new one.
                  </p>

                  <!-- CTA -->
                  <table width='100%' cellpadding='0' cellspacing='0'>
                    <tr>
                      <td align='center' style='padding-bottom:28px;'>
                        <a href='{resetLink}'
                           style='background:linear-gradient(135deg,#6366f1,#8b5cf6);
                                  color:#ffffff;text-decoration:none;
                                  padding:16px 44px;border-radius:12px;
                                  font-size:16px;font-weight:700;
                                  display:inline-block;
                                  box-shadow:0 4px 20px rgba(99,102,241,0.4);'>
                          Reset My Password →
                        </a>
                      </td>
                    </tr>
                  </table>

                  <!-- Warning box -->
                  <table width='100%' cellpadding='0' cellspacing='0'>
                    <tr>
                      <td style='background:rgba(245,158,11,0.1);
                                 border:1px solid rgba(245,158,11,0.3);
                                 border-radius:12px;padding:16px 18px;
                                 margin-bottom:20px;'>
                        <p style='color:#fbbf24;font-size:13px;
                                  margin:0;line-height:1.6;'>
                          ⚠️ <strong>This link expires in 1 hour.</strong>
                          If you did not request a password reset,
                          please ignore this email — your account is safe.
                        </p>
                      </td>
                    </tr>
                  </table>

                  <br/>

                  <!-- Fallback link -->
                  <p style='color:rgba(255,255,255,0.3);font-size:12px;
                            text-align:center;margin:16px 0 0;
                            word-break:break-all;'>
                    If the button does not work, copy this link:<br/>
                    <span style='color:#a5b4fc;'>{resetLink}</span>
                  </p>

                </td>
              </tr>

              <!-- Card footer -->
              <tr>
                <td style='background:rgba(255,255,255,0.03);
                           border-top:1px solid rgba(255,255,255,0.07);
                           padding:20px 40px;text-align:center;'>
                  <p style='color:rgba(255,255,255,0.3);font-size:13px;
                            margin:0 0 6px;'>
                    You received this because a password reset was requested
                    for your SmartSpend account.
                  </p>
                  <p style='color:rgba(255,255,255,0.2);font-size:12px;margin:0;'>
                    © {DateTime.Now.Year} SmartSpend · Built for South Africa 🇿🇦
                  </p>
                </td>
              </tr>

            </td>
          </tr>

          <tr><td style='height:32px;'></td></tr>

        </table>
      </td>
    </tr>
  </table>

</body>
</html>";
        }
    }
}