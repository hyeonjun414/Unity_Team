using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : ActionCommand
{
    Character player;
    public CharacterAction(Character player)
    {
        this.player = player;
    }
    public override void Execute()
    {
        Attack();
        Block();
        UseItem();
        ChangeItemSlot();
    }
    private void Attack()
    {

    }
    private void Block()
    {

    }
    private void UseItem()
    {

    }
    private void ChangeItemSlot()
    {

    }
}
