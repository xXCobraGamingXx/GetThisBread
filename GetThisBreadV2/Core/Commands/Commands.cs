﻿using Discord.WebSocket;
using System;
using Discord;
using System.Threading.Tasks;
using Discord.Commands;
using System.Linq;


namespace GetThisBread.Core.Commands

    //Commands for staff in the discord.

     
{
    
    public class Staff : ModuleBase


    {


        [Command("userinfo"), Summary("Returns the users profile info")]
        [Alias("user")]
        [RequireUserPermission(Discord.GuildPermission.KickMembers)]
        public async Task UserInfo(SocketGuildUser user = null)
        {
          

            EmbedBuilder Embed = new EmbedBuilder();

            if (user == null)
            {
                await Context.Channel.SendMessageAsync(":x: Whoops, you didn't provide a username or a mention! **Use b! user <@user>**");
                return;
            }
            if (user.IsBot)
            {
                await Context.Channel.SendMessageAsync(":x: Sorry but I cannot provide info for a bots as they are not users!");
                return;
            }

            //Embed for userinfo

            var userCreate = user.CreatedAt.LocalDateTime.ToString("dddd, dd MMMM yyyy");

            Embed.AddField("Profile created on:", "\n" + userCreate, true);
            Embed.AddField("User dicriminator:", "\n" + user.Discriminator, false);
            Embed.AddField("Users status:", user.Status, true);
            Embed.WithColor(17, 0, 255);
            Embed.WithFooter($"Action was performed by {Context.User.Username}.");
            Embed.WithTitle($"{user.Username}'s info has been provided.");

            Embed.WithFields();
            Embed.WithThumbnailUrl(user.GetAvatarUrl());

            await Context.Channel.SendMessageAsync("yoink", false, Embed.Build());


        }

        [Command("Purge"), Remarks("Purge the messages in the channel")]
        [Alias("purge")]
        [RequireUserPermission(Discord.GuildPermission.ManageMessages)]

        public async Task Clear(int amountOfMessagesToDelete)
        {
            await (Context.Message.Channel as SocketTextChannel).DeleteMessagesAsync(await Context.Message.Channel.GetMessagesAsync(amountOfMessagesToDelete + 1).FlattenAsync());

            await Context.Channel.SendMessageAsync("Messages have been purged!");

        }




        [Command("UserPurge"), Summary("Purges a specified users messages")]
        [Alias("userp", "userdel")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clear(SocketGuildUser user = null, int amountOfMessagesToDelete = 20)
        {

            if (user == null)
            {
                await Context.Channel.SendMessageAsync(":x: You didn't specify a user to purge! @ them or type their username!");
                return;
            }



            if (user == Context.User)
                amountOfMessagesToDelete++; //Because it will count the purge command as a message

            var messages = await Context.Message.Channel.GetMessagesAsync(amountOfMessagesToDelete).FlattenAsync();

            var result = messages.Where(x => x.Author.Id == user.Id && x.CreatedAt >= DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(14)));

            await (Context.Message.Channel as SocketTextChannel).DeleteMessagesAsync(result);

            await Context.Channel.SendMessageAsync("Users messages have been purged!");

            

        }

        [Command("Kick"), Summary("Kicks the user specified")]
        [Alias("kick", "k")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user = null, [Remainder] string reason = "Reason not provided.")
        {

            EmbedBuilder Embed = new EmbedBuilder();
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(":x: You didn't specify a user!");
                return;
            }

            if (user.IsBot)
            {
                await Context.Channel.SendMessageAsync(":x: If you want to kick bots do it manually! *This is a security feature*");
                return;
            }

            if (reason == "Reason not provided.")
            {
                await Context.Channel.SendMessageAsync(":x: You didn't provide a reason!");
                return;
            }

            //If the user was mentioned and isn't a bot this will execute.

            await user.KickAsync();

            //Shows the date the user was kicked on
            var date = Context.Message.CreatedAt.ToString("dddd, dd MMMM yyyy");

            //This is the embed for the kick
            Embed.WithTitle("User kicked information.");
            Embed.AddField("Username:", user.Username, true);
            Embed.AddField("Kicked by:", Context.User.Mention, true);
            Embed.AddField("Kicked on:", date, true);
            Embed.AddField("Reason:", reason, true);
            Embed.WithColor(255, 30, 6);
            Embed.WithThumbnailUrl(user.GetAvatarUrl());

            await Context.Channel.SendMessageAsync($"{user.Username} was kicked! Providing info of the kick...", false, Embed.Build());


            await user.SendMessageAsync($"You were kicked with the reason:`{reason}`. You are allowed to join back, just know there will be repurcussions due to your behavior. \n " +
                $"Be on good behavior and you will not recieve a kick.  Just know that next time you break a rule or bad mouth someone you will be muted.  If you enter DMs you will be banned.");

        }


        [Command("Ban"), Summary("Bans the user specified")]
        [Alias("ban", "b")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser user = null, [Remainder] string reason = "Reason wasn't provided.")
        {

            if (user == null)
            {
                await Context.Channel.SendMessageAsync(":x: You didn't specify a user. Please mention them or type their name.");
                return;
            }

            if (user.IsBot)
            {
                await Context.Channel.SendMessageAsync(":x: You can't ban a bot. If you want it gone you can kick it or remove it manually.");
                return;
            }

            

            await user.BanAsync();
            await user.SendMessageAsync($"You were banned with the reason:`{reason}`.\n " +
                $"If you would like to appeal");


        }

        [Command("Mute"), Summary("Mute a specified user")]
        [Alias("mute", "M")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Mute(SocketGuildUser user = null, [Remainder] string reason = "Reason not provided.")
        {
            EmbedBuilder Embed = new EmbedBuilder();

            if (user == null)
            {
                await Context.Channel.SendMessageAsync(":x: Hey! You didn't specify a user to mute! Please mention them or type their name with discriminator.");
                return;
            }

            if (user.IsBot)
            {
                await Context.Channel.SendMessageAsync(":x: Sorry but you can't mute bots.");
                return;
            }


            ulong roleID = 693626452687716392;
            var role = Context.Guild.GetRole(roleID);
            await user.AddRoleAsync(role);


            var date = Context.Message.CreatedAt.ToString("dddd, dd MMMM yyyy");
            //This is the embed for the mute.
            Embed.WithTitle("User kicked information.");
            Embed.AddField("Username:", user.Username, true);
            Embed.AddField("Muted by:", Context.User.Mention, true);
            Embed.AddField("Muted on:", date, true);
            Embed.AddField("Reason:", reason, true);
            Embed.WithColor(255, 30, 6);
            Embed.WithThumbnailUrl(user.GetAvatarUrl());

            await Context.Channel.SendMessageAsync($":white_check_mark: {user.Username} was muted! Providing info of the mute...", false, Embed.Build());
            await user.SendMessageAsync($"You were muted with the reason `{reason}`.  Your mute will last as long as staff see fit.  We keep track of when we mute and when the time is up.");

        }


        [Command("Unmute"), Summary("Unmute a specified user.")]
        [Alias("unmute", "um")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UnMute(SocketGuildUser user = null)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync($":x:{user.Mention} You didn't specify a user to unmute! Please @ them or type in their name.");
                return;
            }

            if (user.IsBot)
            {
                await Context.Channel.SendMessageAsync(":x: Sory but bots can't be affected by both mute and unmute.");
                return;
            }

            ulong roleID = 693626452687716392;
            var role = Context.Guild.GetRole(roleID);
            await user.RemoveRoleAsync(role);
            await Context.Channel.SendMessageAsync($":white_check_mark: User {user.Mention} has been unmuted!");



        }



        
    }
   



}












    



