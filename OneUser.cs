namespace TelegramMentionApp;

public class OneUser
{
    public void Start()
    {
        OneUsers();
    }

    public void OneUsers()
    {

        /*class TelegramUserClient
{
    static WTelegram.Client client;
    //2771388374 Mafia Game yangisi
    //3038396242 Mafia Game eskisi
    static async Task Main(string[] args)
    {
        client = new WTelegram.Client(Config);
        var user = await client.LoginUserIfNeeded();
        Console.WriteLine($"Salom, {user.first_name}!");

        await ListAllGroups();

        Console.WriteLine("\nGuruh ID ni kiriting:");
        string input = Console.ReadLine();
        if (long.TryParse(input, out long chatId))
        {
            await MentionAllMembers(chatId);
        }
        else
        {
            Console.WriteLine("Noto'g'ri ID!");
        }

        //OneUser user = new OneUser();

    }

    static async Task ListAllGroups()
    {
        try
        {
            Console.WriteLine("\n=== SUPERGURUHLARINGIZ ===\n");
            var chats = await client.Messages_GetAllChats();
            foreach (var chat in chats.chats.Values)
            {
                if (chat is Channel channel && channel.IsGroup)
                {
                    Console.WriteLine($"👥 {channel.Title}");
                    Console.WriteLine($" ID: {channel.id}");
                    Console.WriteLine($" Username: @{channel.username ?? "yo'q"}");
                    Console.WriteLine($" Turi: Superguruh");
                    Console.WriteLine();
                }
                else if (chat is Chat regularChat)
                {
                    Console.WriteLine($"👥 {regularChat.Title} (Oddiy guruh)");
                    Console.WriteLine($" ID: {regularChat.id}");
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
        }
    }

    static string Config(string what)
    {
        switch (what)
        {
            case "api_id": return "31397769";
            case "api_hash": return "d73908f18fc54388760a4135881ea26e";
            case "phone_number": return "+998331356933";

            case "verification_code":
                Console.Write("Telegram'dan kelgan kodni kiriting: ");
                return Console.ReadLine();
            case "password":
                Console.Write("2FA parolingiz (agar bo'lsa): ");
                return Console.ReadLine();
            default: return null;
        }
    }

    static async Task MentionAllMembers(long chatId)
    {
        try
        {
            var chats = await client.Messages_GetAllChats();
            if (!chats.chats.TryGetValue(chatId, out var chatBase))
            {
                Console.WriteLine("Guruh topilmadi!");
                return;
            }

            if (chatBase is not Channel channel)
            {
                Console.WriteLine("Bu superguruh emas!");
                return;
            }

            if (channel.flags.HasFlag(Channel.Flags.broadcast))
            {
                Console.WriteLine("Bu oddiy kanal — superguruh bo'lishi kerak! (broadcast = false bo'lishi kerak)");
                return;
            }

            Console.WriteLine($"Superguruh: {channel.Title}");

            var participants = await client.Channels_GetParticipants(
                channel: channel,
                filter: new ChannelParticipantsRecent(),
                offset: 0,
                limit: 200
            );

            var users = new List<User>();
            int offset = 0;
            while (participants?.users?.Count > 0)
            {
                users.AddRange(participants.users.Values.OfType<User>());
                offset += participants.users.Count;
                if (participants.users.Count < 200) break;

                participants = await client.Channels_GetParticipants(
                    channel: channel,
                    filter: new ChannelParticipantsRecent(),
                    offset: offset,
                    limit: 200
                );
            }

            Console.WriteLine($"Jami {users.Count} ta foydalanuvchi topildi.\n");

            int sent = 0;
            foreach (var user in users)
            {
                if (user.IsBot) continue;

                try
                {
                    string messageText;
                    InputMessageEntityMentionName[] entities = null;

                    if (!string.IsNullOrEmpty(user.username))
                    {
                        messageText = $"@{user.username}";
                        await client.SendMessageAsync(channel, messageText);
                    }
                    else
                    {
                        string name = user.first_name ?? "Foydalanuvchi";
                        messageText = $"{name} ";
                        entities = new[] {
                            new InputMessageEntityMentionName {
                                offset = 0,
                                length = name.Length,
                                user_id = new InputUser(user.id, user.access_hash)
                            }
                        };

                        await client.SendMessageAsync(
                            channel,
                            messageText,
                            entities: entities
                        );
                    }

                    sent++;
                    Console.WriteLine($"[{sent}/{users.Count}] Chaqirildi: {messageText.Trim()}");

                    await Task.Delay(1200);
                }
                catch (RpcException ex) when (ex.Code == 420 || ex.Message.Contains("FLOOD_WAIT"))
                {
                    int waitSeconds = ex.X > 0 ? ex.X : 60;
                    Console.WriteLine($"FloodWait: {waitSeconds} soniya kutish...");
                    await Task.Delay(waitSeconds * 1000 + 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Xato (user {user.id}): {ex.Message}");
                    await Task.Delay(2000);
                }
            }

            Console.WriteLine($"\nMuvaffaqiyat! {sent} ta foydalanuvchi alohida xabar bilan chaqirildi.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Umumiy xatolik: {ex.Message}\n{ex.StackTrace}");
        }
    }
}*/
    }
}

