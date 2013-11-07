new ScriptObject(BottomPrint);

exec("./profiles.cs");
exec("./bottomPrint.gui");

function BottomPrint::onStart(%this) {
   PlayGui.add(BottomPrintDlg);
}
