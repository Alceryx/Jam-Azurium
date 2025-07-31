extends Upgrade

func _process(delta: float) -> void:
	super._process(delta)
	Info.text = "Maxed" if Maxed else "+%s Max HP" % str(Database[CurrentTier].Addition)

func Update() -> void:
	GameManager.GM.ButtonHP += Database[CurrentTier].Addition
	GameManager.GM.ButtonMaxHP += Database[CurrentTier].Addition
	super.Update()
