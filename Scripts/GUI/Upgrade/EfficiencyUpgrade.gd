extends Upgrade
class_name EfficiencyUpgrade

func _process(delta):
	super._process(delta)
	if Maxed:
		Info.text = "Maxed"
	else:
		Info.text = "+%d Efficiency" % Database[CurrentTier].Addition

func Update():
	GameManager.GM.Efficiency += Database[CurrentTier].Addition
	super.Update()
