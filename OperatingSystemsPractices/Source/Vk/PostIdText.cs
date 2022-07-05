using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace OperatingSystemsPractices.Source.Vk
{
    public class PostIdText
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public static List<PostIdText> Create(List<Post> posts)
        {
            List<PostIdText> postsIdText = new List<PostIdText>();
            foreach (var post in posts)
            {
                postsIdText.Add(new PostIdText() { Id = post.Id, Text = post.Text });
            }
            return postsIdText.Where(obj => obj.Text != string.Empty).ToList();
        }

        public static void CreateJsonFile(string filePath, List<PostIdText> posts)

        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(posts, Json.GetOptions()));
        }

        public static void AddInJsonFile(string filePath, List<PostIdText> posts, bool checkExists = true)
        {
            if (!File.Exists(filePath))
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<PostIdText> existsPosts = JsonSerializer.Deserialize<List<PostIdText>>(File.ReadAllText(filePath), Json.GetOptions());
            if (existsPosts == null || !existsPosts.Any())
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<PostIdText> newPosts;
            if (checkExists)
                newPosts = posts.Except(existsPosts).ToList();
            else
                newPosts = posts;

            if (newPosts.Any())
            {
                FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);

                stream.Seek(-3, SeekOrigin.End);
                writer.Write(',' + JsonSerializer.Serialize(newPosts, Json.GetOptions()).Substring(1));
                writer.Close();
                stream.Close();
            }
        }

        public static List<PostIdText> ReadFromJsonFile(string filePath)
        {
            return JsonSerializer.Deserialize<List<PostIdText>>(File.ReadAllText(filePath), Json.GetOptions());
        }

        public override bool Equals(object obj)
        {
            if (obj is PostIdText postIdText) return Id == postIdText.Id;
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
