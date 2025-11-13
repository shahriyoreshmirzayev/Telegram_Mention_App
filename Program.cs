using System;
using System.Linq;
using System.Threading.Tasks;
using TL;
using WTelegram;

class TelegramUserClient
{
    static WTelegram.Client client;

    static async Task Main(string[] args)
    {
        // API ma'lumotlarini kiriting (https://my.telegram.org/apps dan oling)
        client = new WTelegram.Client(Config);

        // Login qiling
        var user = await client.LoginUserIfNeeded();
        Console.WriteLine($"Salom, {user.first_name}!");

        // BIRINCHI: Barcha guruhlaringizni ko'rish
        await ListAllGroups();

        Console.WriteLine("\n\nGuruh ID ni kiriting (yuqoridagi ro'yxatdan):");
        string input = Console.ReadLine();

        if (long.TryParse(input, out long chatId))
        {
            // A'zolarni mention qilish
            await MentionAllMembers(chatId);
        }
        else
        {
            Console.WriteLine("Noto'g'ri ID!");
        }
    }

    // Barcha guruhlarni ko'rsatish
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
                    Console.WriteLine($"📢 {channel.Title}");
                    Console.WriteLine($"   ID: {channel.id}");
                    Console.WriteLine($"   Username: @{channel.username ?? "yo'q"}");
                    Console.WriteLine($"   Turi: {(channel.IsChannel ? "Kanal" : "Superguruh")}");
                    Console.WriteLine();
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

    // Konfiguratsiya (API ID va API Hash kiritish)
    static string Config(string what)
    {
        switch (what)
        {
            case "api_id": return "31397769"; // https://my.telegram.org/apps
            case "api_hash": return "d73908f18fc54388760a4135881ea26e";
            case "phone_number": return "+998331356933"; // Telefon raqamingiz
            case "verification_code":
                Console.Write("Telegram'dan kelgan kodni kiriting: ");
                return Console.ReadLine();
            case "password":
                Console.Write("2FA parolingiz (agar bo'lsa): ");
                return Console.ReadLine();
            default: return null;
        }
    }

    // Barcha a'zolarni mention qilish
    static async Task MentionAllMembers(long chatId)
    {
        try
        {
            // Chatni olish
            var chats = await client.Messages_GetAllChats();

            if (!chats.chats.TryGetValue(chatId, out var chatBase))
            {
                Console.WriteLine("Guruh topilmadi!");
                return;
            }

            Console.WriteLine($"Guruh: {chatBase.Title}");

            // Channel sifatida cast qilish
            if (chatBase is not Channel channel)
            {
                Console.WriteLine("Bu kanal/superguruh emas!");
                return;
            }

            // Guruh a'zolarini olish
            var participants = await client.Channels_GetParticipants(
                channel: channel,
                filter: new ChannelParticipantsRecent(),
                offset: 0,
                limit: 200
            );

            Console.WriteLine($"Jami {participants.users.Count} a'zo topildi");

            // Mention uchun xabar yaratish
            var messageText = "📢 Diqqat barcha a'zolar!\n\n";
            var entities = new System.Collections.Generic.List<MessageEntity>();

            int offset = messageText.Length;

            foreach (var userBase in participants.users.Values)
            {
                if (userBase is User user && !user.IsBot)
                {
                    string displayName = !string.IsNullOrEmpty(user.first_name)
                        ? user.first_name
                        : user.username ?? "User";

                    string mention = $"{displayName} ";
                    messageText += mention;

                    // Mention entity yaratish
                    entities.Add(new InputMessageEntityMentionName
                    {
                        offset = offset,
                        length = mention.TrimEnd().Length,
                        user_id = new InputUser(user.id, user.access_hash)
                    });

                    offset += mention.Length;
                }
            }

            // Xabarni yuborish
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

 