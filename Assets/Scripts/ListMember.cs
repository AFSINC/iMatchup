using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListMember : MonoBehaviour {
//	public GameObject pfbMember;

	private void listUpdate() {
/*
		for (int i = 0; i < MemberManager.getMemberCount (); i++) {
			Transform pt = Instantiate (pfbMember).transform;
			pt.SetParent (this.transform, false);

			MemberManager.posMember (i);
			string sname = MemberManager.nameKaji_family + " " + MemberManager.nameKaji_first;
			pt.GetComponentInChildren <Text> ().text = sname;

			if (MemberManager.gender == 0) {
				pt.GetComponentInChildren <Image> ().color = Colors.male;
			} else {	
				pt.GetComponentInChildren <Image> ().color = Colors.female;
			}
		}
*/
/*
		for (int ii=0; ii<2; ii++) {
			Transform pt = Instantiate (pfbMember).transform;
			pt.SetParent (this.transform, false);
			pt.GetComponentInChildren <Text>().text = "test";
		}
*/
	}

	void Start () {
//		Destroy (this.transform.FindChild ("ListMember").gameObject); // Test表示のメンバ削除
//		listUpdate ();
	}
}
