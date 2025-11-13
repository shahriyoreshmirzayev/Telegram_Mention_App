using TL;

class TelegramUserClient
{
    static WTelegram.Client client;

    static async Task Main(string[] args)
    {
        client = new WTelegram.Client(Config);

        var user = await client.LoginUserIfNeeded();
        Console.WriteLine($"Salom, {user.first_name}!");

        await ListAllGroups();

        Console.WriteLine("\n\nGuruh ID ni kiriting (yuqoridagi ro'yxatdan):");
        string input = Console.ReadLine();

        if (long.TryParse(input, out long chatId))
        {
            await MentionAllMembers(chatId);
        }
        else
        {
            Console.WriteLine("Noto'g'ri ID!");
        }
    }

    static async Task ListAllGroups()
    {
        try
        {
            Console.WriteLine("\n=== SIZNING GURUHLARINGIZ ===\n");
            var chats = await client.Messages_GetAllChats();
            foreach (var chat in chats.chats.Values)
            {
                if (chat is Channel channel)
                {
                    if (!channel.IsChannel)
                    {
                        Console.WriteLine($"👥 {channel.Title}");
                        Console.WriteLine($"   ID: {channel.id}");
                        Console.WriteLine($"   Username: @{channel.username ?? "yo'q"}");
                        Console.WriteLine($"   Turi: Superguruh");
                        Console.WriteLine();
                    }
                }
                else if (chat is Chat regularChat)
                {
                    Console.WriteLine($"👥 {regularChat.Title}");
                    Console.WriteLine($"   ID: {regularChat.id}");
                    Console.WriteLine($"   Turi: Oddiy guruh");
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

            Console.WriteLine($"Guruh: {chatBase.Title}");

            if (chatBase is not Channel channel)
            {
                Console.WriteLine("Bu kanal/superguruh emas!");
                return;
            }
            var participants = await client.Channels_GetParticipants(
                channel: channel,
                filter: new ChannelParticipantsRecent(),
                offset: 0,
                limit: 200
            );
            Console.WriteLine($"Jami {participants.users.Count} a'zo topildi");
            var messageText = "📢 Diqqat barcha a'zolar!\n\n";
            var entities = new System.Collections.Generic.List<MessageEntity>();
            int offset = messageText.Length;
            foreach (var userBase in participants.users.Values)
            {
                if (userBase is User user && !user.IsBot)
                {
                    string displayName = !string.IsNullOrEmpty(user.first_name) ? user.first_name : user.username ?? "User"; string mention = $"{displayName} ";
                    messageText += mention;
                    entities.Add(new InputMessageEntityMentionName { offset = offset, length = mention.TrimEnd().Length, user_id = new InputUser(user.id, user.access_hash) });
                    offset += mention.Length;
                }
            }
            await client.SendMessageAsync(channel, messageText, entities: entities.ToArray());
            Console.WriteLine("Xabar muvaffaqiyatli yuborildi!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Xatolik: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
