using System.Collections.Generic;
using NetIrc2;
using NetIrc2.Events;

namespace ShimabuttsIrcBot.Commands
{
    public class UndoneCommand : BotCommand
    {
        protected override void SpecificCommand(ChatMessageEventArgs eventArgs, IrcClient ircClient, Dictionary<string, Project.Project> projects, ShimabuttsRedis redis)
        {
            var splits = eventArgs.Message.ToString().Split(' ');
            if (splits.Length == 3)
            {
                if (projects.ContainsKey(splits[1]))
                {
                    var role = splits[2].ParseStringToRole();
                    if (!role.HasValue)
                    {
                        ircClient.Message("#Piroket", "wtf role is that");
                        return;
                    }
                    projects[splits[1]].SetAsUndone(role.Value);
                    redis.SetRoleUndone(splits[1], role.Value);
                    redis.SetTimeWaiting(splits[1], projects[splits[1]].GetDateTime());
                    var waitingAtRole = projects[splits[1]].WaitingAt();
                    if (waitingAtRole.HasValue)
                    {
                        ircClient.Message("#Piroket", string.Format("{0} is done for {1}. Waiting on {2} - {3}",
                            role,
                            splits[1],
                            projects[splits[1]].WaitingAt(),
                            string.Join(",", projects[splits[1]].CheckProjectForRole(waitingAtRole.Value)))
                            );
                    }
                    else
                    {
                        ircClient.Message("#Piroket", string.Format("{0} is done!", splits[1]));
                    }
                }
                else
                {
                    ircClient.Message("#Piroket", "No project by that name.");
                }
            }
            else
            {
                ircClient.Message("#Piroket", "Usage: .undone [project] [role]");
            }
        }
    }
}