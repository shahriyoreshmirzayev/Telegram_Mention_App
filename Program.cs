using TL;
//2771388374 game yangisi
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

            // BARCHA a'zolarni olish uchun offset bilan ishlash
            var allUsers = new Dictionary<long, User>();
            int offset = 0;
            int limit = 50; // Har bir so'rovda 50 ta a'zo

            while (true)
            {
                var participants = await client.Channels_GetParticipants(
                    channel: channel,
                    filter: new ChannelParticipantsRecent(),
                    offset: offset,
                    limit: limit
                );

                Console.WriteLine($"Yuklanmoqda: {offset} dan {offset + participants.users.Count} gacha...");

                // Foydalanuvchilarni qo'shish
                foreach (var userBase in participants.users.Values)
                {
                    if (userBase is User user && !user.IsBot)
                    {
                        allUsers[user.id] = user;
                    }
                }

                // Agar kamroq foydalanuvchi qaytgan bo'lsa, demak oxirgi bo'lim
                if (participants.users.Count < limit)
                    break;

                offset += participants.users.Count;

                // Telegram API cheklovlarini oldini olish uchun biroz kutish
                await Task.Delay(1000);
            }

            Console.WriteLine($"\nJami {allUsers.Count} ta a'zo topildi!");
            Console.WriteLine("Xabar tayyorlanmoqda...\n");

            // Telegram xabar uzunligi cheklovi: 4096 belgi
            // Agar a'zolar ko'p bo'lsa, bir nechta xabarga bo'lib yuborish kerak
            const int maxMessageLength = 4000; // Biroz zahira qoldiramiz

            var messageText = "📢 Diqqat barcha a'zolar!\n\n";
            var entities = new List<MessageEntity>();
            int currentOffset = messageText.Length;
            int mentionCount = 0;

            foreach (var user in allUsers.Values)
            {
                string displayName = !string.IsNullOrEmpty(user.first_name)
                    ? user.first_name
                    : user.username ?? "User";

                string mention = $"{displayName} ";

                // Agar xabar juda uzun bo'lib ketayotgan bo'lsa, yuborib yangi xabar boshlaymiz
                if (messageText.Length + mention.Length > maxMessageLength)
                {
                    await client.SendMessageAsync(channel, messageText, entities: entities.ToArray());
                    Console.WriteLine($"✅ {mentionCount} ta a'zo yuborildi");

                    // Yangi xabar uchun tayyorlanish
                    await Task.Delay(2000); // Spam deb olinmaslik uchun kutamiz
                    messageText = "📢 Davomi...\n\n";
                    entities.Clear();
                    currentOffset = messageText.Length;
                    mentionCount = 0;
                }

                messageText += mention;
                entities.Add(new InputMessageEntityMentionName
                {
                    offset = currentOffset,
                    length = mention.TrimEnd().Length,
                    user_id = new InputUser(user.id, user.access_hash)
                });

                currentOffset += mention.Length;
                mentionCount++;
            }

            // Oxirgi xabarni yuborish
            if (mentionCount > 0)
            {
                await client.SendMessageAsync(channel, messageText, entities: entities.ToArray());
                Console.WriteLine($"✅ {mentionCount} ta a'zo yuborildi");
            }

            Console.WriteLine($"\n🎉 Jami {allUsers.Count} ta a'zo muvaffaqiyatli mention qilindi!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Xatolik: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}