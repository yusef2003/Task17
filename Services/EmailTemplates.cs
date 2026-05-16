namespace CinemaDashboard.Services
{
    // SRP: Only responsible for building HTML email bodies
    public static class EmailTemplates
    {
        private static string Wrap(string title, string body) => $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <style>
    body {{ font-family: 'Segoe UI', Arial, sans-serif; background:#0d0f14; margin:0; padding:0; }}
    .wrap {{ max-width:560px; margin:40px auto; background:#161a23; border:1px solid #252a3a; border-radius:12px; overflow:hidden; }}
    .header {{ background:#c9972c; padding:28px 32px; }}
    .header h1 {{ margin:0; color:#0a0b0f; font-size:22px; letter-spacing:.04em; }}
    .body {{ padding:32px; color:#c8cfe0; line-height:1.7; font-size:15px; }}
    .body h2 {{ color:#f0f2f8; margin-top:0; }}
    .btn {{ display:inline-block; background:#c9972c; color:#0a0b0f!important; font-weight:700;
            text-decoration:none; padding:14px 32px; border-radius:6px; margin:20px 0; font-size:15px; }}
    .footer {{ padding:16px 32px; border-top:1px solid #252a3a; font-size:12px; color:#4a5568; }}
    .note {{ background:#1c2030; border-left:3px solid #c9972c; padding:12px 16px; border-radius:4px; font-size:13px; color:#8a95b0; }}
  </style>
</head>
<body>
  <div class='wrap'>
    <div class='header'><h1>🎬 Cinema App — {title}</h1></div>
    <div class='body'>{body}</div>
    <div class='footer'>© {DateTime.Now.Year} Cinema App. This email was sent automatically.</div>
  </div>
</body>
</html>";

        public static string ConfirmEmail(string fullName, string confirmUrl) =>
            Wrap("Email Confirmation", $@"
<h2>Hello, {fullName}!</h2>
<p>Thank you for registering. Click the button below to confirm your email address and activate your account.</p>
<a class='btn' href='{confirmUrl}'>✅ Confirm My Email</a>
<div class='note'>This link will expire in 24 hours. If you did not create this account, you can safely ignore this email.</div>");

        public static string ResetPassword(string fullName, string resetUrl) =>
            Wrap("Password Reset", $@"
<h2>Hello, {fullName}!</h2>
<p>We received a request to reset your password. Click the button below to choose a new password.</p>
<a class='btn' href='{resetUrl}'>🔑 Reset My Password</a>
<div class='note'>This link will expire in 1 hour. If you did not request a password reset, you can safely ignore this email.</div>");
    }
}
