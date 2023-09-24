# Setting the Telegram Bot Token for CryptoBardWorkerService

To run CryptoBardWorkerService, follow these simple steps to set your Telegram Bot Token as a system-level environment variable:

1. Open a Command Prompt window.

2. Use this command, replacing `"YOUR_BOT_TOKEN"` with your actual Telegram Bot Token:

   ```bash
   setx CryptoBard_Telegram__BotToken "YOUR_BOT_TOKEN" /M

3. Restart your IDE (Integrated Development Environment) to apply the changes.

That's it! Now your CryptoBardWorkerService can access the Telegram Bot Token.

# Troubleshooting "Access is Denied" Issue

If you encounter an "Access is denied" error (0x80070005) when working with Windows notifications, it may be due to the service running as the system account. To fix this:

1. Open Services Manager (Win + R, type `services.msc`, and press Enter).
2. Find your app's service, right-click, and choose "Properties."
3. In the "Log On" tab, change from "Local System Account" to a user account.
4. Enter the user's credentials and click "Apply" and "OK."
5. Restart the service.

This should resolve the issue. Ensure the user account has necessary permissions for your app.