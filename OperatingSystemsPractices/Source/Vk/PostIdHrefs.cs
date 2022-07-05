using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace OperatingSystemsPractices.Source.Vk
{
    public class PostIdHrefs
    {
        public string Id { get; set; }
        public string[] Hrefs { get; set; }

        public static List<PostIdHrefs> Create(List<Post> posts)
        {
            List<PostIdHrefs> postsIdHrefs = new List<PostIdHrefs>();
            foreach (var post in posts)
            {
                postsIdHrefs.Add(new PostIdHrefs() { Id = post.Id, Hrefs = post.Hrefs });
            }
            return postsIdHrefs.Where(obj => obj.Hrefs != Array.Empty<string>()).ToList();
        }

        public static void CreateJsonFile(string filePath, List<PostIdHrefs> posts)
        {
            File.WriteAllText(filePath, JsonSerializer.Serialize(posts, Json.GetOptions()));
        }

        public static void AddInJsonFile(string filePath, List<PostIdHrefs> posts, bool checkExists = true)
        {
            if (!File.Exists(filePath))
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<PostIdHrefs> existsPosts = JsonSerializer.Deserialize<List<PostIdHrefs>>(File.ReadAllText(filePath), Json.GetOptions());
            if (existsPosts == null || !existsPosts.Any())
            {
                CreateJsonFile(filePath, posts);
                return;
            }

            List<PostIdHrefs> newPosts;
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

        public static List<PostIdHrefs> ReadFromJsonFile(string filePath)
        {
            return JsonSerializer.Deserialize<List<PostIdHrefs>>(File.ReadAllText(filePath), Json.GetOptions());
        }

        public override bool Equals(object obj)
        {
            if (obj is PostIdHrefs postIdHrefs) return Id == postIdHrefs.Id;
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
