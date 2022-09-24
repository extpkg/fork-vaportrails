using System.Collections.Generic;
using System;
using UnityEngine;
using XNode;

[NodeWidth(270)]
public class AttackNode : CombatNode {
	public AttackData attackData;

    [Input(backingValue=ShowBackingValue.Never)] 
    public AttackLink input;

    [Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
    public AttackLink[] links;

    List<Tuple<AttackLink, CombatNode>> directionalLinks = new List<Tuple<AttackLink, CombatNode>>();
    CombatNode anyDirectionNode = null;

    [Output(backingValue=ShowBackingValue.Never, connectionType=ConnectionType.Override)]
    public AttackLink onHit;

    [HideInInspector]
    public float timeOffset = 0;

    override public void OnNodeEnter() {
        base.OnNodeEnter();
        if (attackData != null) {
            attackGraph.animator.Play(attackData.name, layer:0, normalizedTime:timeOffset);
        }
        timeOffset = 0;
    }

    public string GetAnimationStateName() {
        return attackData.name;
    }

    override public void NodeUpdate(int currentFrame, float clipTime, AttackBuffer buffer) {
        base.NodeUpdate(currentFrame, clipTime, buffer);

        if (attackLanded && CanMoveNode(nameof(onHit))) {
            CombatNode node = GetNode(nameof(onHit)).Connection.node as CombatNode;
            attackGraph.MoveNode(node);
            return;
        } else if (buffer.Ready()) {
            if (currentFrame>=attackData.IASA) {
                MoveNextNode(buffer, allowReEntry: true);
                return;
            } else if (attackLanded) {
                MoveNextNode(buffer);
                return;
            }
        }
        
        if (currentFrame>=attackData.IASA && attackGraph.inputManager.HasHorizontalInput()) {
            attackGraph.ExitGraph();
            return;
        }

        attackGraph.animator.SetBool("Actionable", currentFrame>=attackData.IASA);
    }

    override public void OnNodeExit() {
        base.OnNodeExit();
        timeOffset = 0;
    }
 
    protected void MoveNextNode(AttackBuffer buffer, bool allowReEntry=false) {
        CombatNode next = GetNextNode(buffer);
        if (next != null) {
            attackGraph.MoveNode(next);
            return;
        }

        if (allowReEntry) {
            attackGraph.EnterGraph();
        }
    }

    virtual public CombatNode GetNextNode(AttackBuffer buffer) {
        return MatchAttackNode(buffer, this.links);
    }

    // directional attacks are prioritized in order, then the first any-directional link is used
    protected CombatNode MatchAttackNode(AttackBuffer buffer, AttackLink[] attackLinks, string portListName="links") {
        directionalLinks.Clear();
        anyDirectionNode = null;
		bool consumedAttack = false;

		// keep looking through the buffer for the next action
		while (buffer.Ready() && !consumedAttack) {
			BufferedAttack attack = buffer.Consume();
			for (int i=0; i<attackLinks.Length; i++) {
				AttackLink link = attackLinks[i];
				if (link.type==attack.type && attack.HasDirection(link.direction)) {
					consumedAttack = true;
					CombatNode next = GetNode(portListName+" "+i).Connection.node as CombatNode;
					if (next.Enabled()) {
						if (link.direction != AttackDirection.ANY) {
							directionalLinks.Add(new Tuple<AttackLink, CombatNode>(link, next));
						} else if (anyDirectionNode == null) {
							anyDirectionNode = next;
						}
					}
                    break;
				}
			}
		}

        if (directionalLinks.Count > 0) {
            return directionalLinks[0].Item2;
        }

        if (anyDirectionNode != null) {
            return anyDirectionNode;
        }

        return null;
    }
}

/*
#if UNITY_EDITOR

// highlight the current node
// unfortunately doesn't always update in time, but oh well
[CustomNodeEditor(typeof(AttackNode))]
public class AttackNodeEditor : NodeEditor {
    private AttackNode attackNode;
    private static GUIStyle editorLabelStyle;

    public override void OnBodyGUI() {
        attackNode = target as AttackNode;

        if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
        if (attackNode.active) EditorStyles.label.normal.textColor = Color.cyan;
        base.OnBodyGUI();
        EditorStyles.label.normal = editorLabelStyle.normal;
    }
}

#endif
*/
