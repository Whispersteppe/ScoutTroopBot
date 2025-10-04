using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoutTroopBot.Workflows;

public interface UserWorkflows
{
    void UpdateRank();
    void UpdatePosition();
    void RequestMeritBadge();
    void ChangePatrol();

    void NewUserGatherInfo();
    void UpdateNickname();
}
