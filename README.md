# Setting the Telegram Bot Token for CryptoBardWorkerService

Follow these steps to set your Telegram Bot Token as a system-level environment variable for CryptoBardWorkerService:

1. Open a Command Prompt window.

2. Run this command, replacing `"YOUR_BOT_TOKEN"` with your actual Telegram Bot Token:

   ```bash
   setx CryptoBard_AppSettings__TelegramBotToken "YOUR_BOT_TOKEN" /M

3. Restart your IDE (Integrated Development Environment) to apply the changes.

# Troubleshooting "Access is Denied" Issue

If you encounter an "Access is denied" error (0x80070005) related to Windows notifications, it may be due to the service running as the system account. To resolve this:

1. Open Services Manager (Win + R, type `services.msc`, and press Enter).
2. Locate your app's service, right-click it, and select "Properties."
3. In the "Log On" tab, switch from "Local System Account" to a user account.
4. Enter the user's credentials, click "Apply," and then "OK."
5. Restart the service.