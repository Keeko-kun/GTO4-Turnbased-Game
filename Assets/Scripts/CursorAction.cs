using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorAction : MonoBehaviour
{

    public ActionMode mode;
    public GameObject statsPanel;
    public GameObject buttonsPanel;
    public GameObject predictionsPanel;

    private bool performingAction;
    private MoveCursor cursor;
    private GameObject unit;
    private GameObject target;
    private AllWalkableTiles walkableTiles;
    private fadePanel unitStats;
    private fadePanel buttonsFade;
    private fadePanel predictionsFade;
    public fadePanel victoryFade;
    public fadePanel gameOverFade;
    public fadePanel blackness;
    private ChangeStats updatePanel;
    private ChangeWeapons updateWeapons;
    public ChangePrediction updatePrediction { get; private set; }
    public AttackSequence attackSequence { get; private set; }

    public AllWalkableTiles WalkableTiles { get { return walkableTiles; } }

    private void Start()
    {
        mode = ActionMode.SelectTile;
        performingAction = false;
        cursor = GetComponent<MoveCursor>();
        attackSequence = GetComponent<AttackSequence>();
        walkableTiles = new AllWalkableTiles();
        walkableTiles.Map = cursor.map.GetMap();
        unitStats = statsPanel.GetComponent<fadePanel>();
        buttonsFade = buttonsPanel.GetComponent<fadePanel>();
        predictionsFade = predictionsPanel.GetComponent<fadePanel>();
        updatePanel = statsPanel.GetComponent<ChangeStats>();
        updateWeapons = buttonsPanel.GetComponent<ChangeWeapons>();
        updatePrediction = predictionsPanel.GetComponent<ChangePrediction>();

        blackness.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("joystick button 1") && mode != ActionMode.EnemyTurn && mode != ActionMode.Victory)
        {
            ResetToSelectTile();
        }
        if (mode == ActionMode.SelectWeapon && mode != ActionMode.EnemyTurn)
        {
            SelectWeapon();
            return;
        }
        if (Input.GetKeyDown("joystick button 0") && mode != ActionMode.EnemyTurn && mode != ActionMode.Victory)
        {
            switch (mode)
            {
                case ActionMode.MoveUnit:
                    StartCoroutine(MoveUnit((int)cursor.GetCurrentTile.PosX, (int)cursor.GetCurrentTile.PosZ));
                    break;
                case ActionMode.SelectTile:
                    SelectTile();
                    break;
                case ActionMode.SelectAction:
                    SelectAction();
                    break;
                case ActionMode.SelectTarget:
                    SelectTarget();
                    break;
                case ActionMode.ConfirmBattle:
                    StartCoroutine(ConfirmBattle(unit.GetComponent<Unit>(), target.GetComponent<Unit>()));
                    break;
                case ActionMode.ViewEnemyStats:
                    ResetToSelectTile();
                    break;
            }
        }

        if (Input.GetKey("joystick button 6"))
        {
            Time.timeScale = 3;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void SelectWeapon()
    {
        if (Input.GetKeyDown("joystick button 0") && updateWeapons.buttons[0].text != "")
        {
            unit.GetComponent<Unit>().Weapon = unit.GetComponent<Unit>().stats.Attacks[0];
            SelectWeaponToAttackWith();
        }
        else if (Input.GetKeyDown("joystick button 2") && updateWeapons.buttons[1].text != "")
        {
            unit.GetComponent<Unit>().Weapon = unit.GetComponent<Unit>().stats.Attacks[1];
            SelectWeaponToAttackWith();
        }
        else if (Input.GetKeyDown("joystick button 3") && updateWeapons.buttons[2].text != "")
        {
            unit.GetComponent<Unit>().Weapon = unit.GetComponent<Unit>().stats.Attacks[2];
            SelectWeaponToAttackWith();
        }
    }

    private void SelectTile()
    {
        if (cursor.GetCurrentTile.Unit != null)
        {
            if (!cursor.GetCurrentTile.Unit.GetComponent<Unit>().playerUnit)
            {
                mode = ActionMode.ViewEnemyStats;
                unit = cursor.GetCurrentTile.Unit;
                updatePanel.UpdateUI(cursor.GetCurrentTile.Unit.GetComponent<Unit>());
                unitStats.visible = true;
                unit = null;
            }
            else
            {
                unit = cursor.GetCurrentTile.Unit;
                if (!unit.GetComponent<Movement>().walking)
                {
                    mode = ActionMode.SelectAction;
                    cursor.chooseAction.GetComponent<fadePanel>().visible = true;
                    cursor.chooseAction.GetComponent<ChangeActions>().UpdateUI(unit.GetComponent<Unit>().HasMoved, unit.GetComponent<Unit>().HasAttacked);
                    updatePanel.UpdateUI(unit.GetComponent<Unit>());
                    unitStats.visible = true;
                }
            }
        }
    }

    private void SelectTarget()
    {
        if (cursor.GetCurrentTile.Piece.GetComponent<Outline>().color == (int)SelectColors.OutOfRange)
        {
            ResetToSelectTile();
            return;
        }
        if (cursor.GetCurrentTile.Unit != null)
        {
            mode = ActionMode.ConfirmBattle;
            target = cursor.GetCurrentTile.Unit;
            attackSequence.PredictOutcome(unit.GetComponent<Unit>(), target.GetComponent<Unit>(), unit.GetComponent<Unit>().Weapon, updatePrediction);
            predictionsFade.visible = true;
            unit.GetComponentInChildren<Outline>().enabled = false;
            walkableTiles.DecolorTiles();
        }
    }

    public IEnumerator ConfirmBattle(Unit unit, Unit target)
    {
        unit.GetComponent<Unit>().HasAttacked = true;
        mode = ActionMode.WaitForBattle;
        List<AttackTurn> turns = new List<AttackTurn>();
        if (int.Parse(updatePrediction.hitAttacker.text) > 0) turns.Add(new AttackTurn(unit.GetComponent<Unit>(), target.GetComponent<Unit>()));
        if (int.Parse(updatePrediction.hitDefender.text) > 0) turns.Add(new AttackTurn(target.GetComponent<Unit>(), unit.GetComponent<Unit>()));
        if (updatePrediction.twiceAttacker.enabled) turns.Add(new AttackTurn(unit.GetComponent<Unit>(), target.GetComponent<Unit>()));
        if (updatePrediction.twiceDefender.enabled) turns.Add(new AttackTurn(target.GetComponent<Unit>(), unit.GetComponent<Unit>()));

        predictionsFade.visible = false;

        yield return StartCoroutine(attackSequence.ExecuteBattle(turns, unit.GetComponent<Unit>()));

        ResetToSelectTile();
    }

    private void SelectAction()
    {
        switch (cursor.chooseAction.currentAction)
        {
            case CurrentAction.Move:
                if (!unit.GetComponent<Unit>().HasMoved && !unit.GetComponent<Unit>().HasAttacked) SelectActionMoveUnit();
                else return;
                break;
            case CurrentAction.Back:
                ResetToSelectTile();
                break;
            case CurrentAction.Attack:
                if (!unit.GetComponent<Unit>().HasAttacked) SelectActionAttack();
                else return;
                break;
            case CurrentAction.EndTurn:
                mode = ActionMode.EnemyTurn;
                StartCoroutine(SwitchTurn());
                break;
        }

        unitStats.visible = false;
        cursor.chooseAction.GetComponent<fadePanel>().visible = false;
        cursor.chooseAction.currentAction = CurrentAction.Move;
    }

    private void SelectActionMoveUnit()
    {
        walkableTiles.Unit = unit;
        walkableTiles.ReachableTiles(true, false);
        unit.GetComponentInChildren<Outline>().enabled = true;
        mode = ActionMode.MoveUnit;
    }

    private IEnumerator MoveUnit(int x, int z)
    {
        GameObject copyOfUnit = unit;
        ResetToSelectTile();

        if (cursor.GetCurrentTile.Piece.GetComponent<Outline>().color == (int)SelectColors.OutOfRange ||
            copyOfUnit.GetComponent<Movement>().walking)
        {
            ResetToSelectTile();
            yield break;
        }


        cursor.map.SetUnit((int)copyOfUnit.GetComponent<Movement>().currentTile.PosX, (int)copyOfUnit.GetComponent<Movement>().currentTile.PosZ, null);
        cursor.map.SetUnit(x, z, copyOfUnit);
        copyOfUnit.GetComponent<Unit>().HasMoved = true;
        yield return StartCoroutine(copyOfUnit.GetComponent<Movement>().StartMovement(cursor.GetCurrentTile));

    }

    private void SelectActionAttack()
    {
        buttonsFade.visible = true;
        updateWeapons.UpdatUI(unit.GetComponent<Unit>().stats.Attacks);
        mode = ActionMode.SelectWeapon;
    }

    private void SelectWeaponToAttackWith()
    {
        mode = ActionMode.SelectTarget;
        buttonsFade.visible = false;
        unit.GetComponentInChildren<Outline>().enabled = true;
        walkableTiles.Unit = unit;
        walkableTiles.ReachableTiles(false, false);
    }

    private IEnumerator SwitchTurn()
    {
        mode = ActionMode.EnemyTurn;

        foreach (GameObject unit in GetComponent<PlayerSession>().playerUnits)
        {
            unit.GetComponent<Unit>().HasMoved = false;
            unit.GetComponent<Unit>().HasAttacked = false;
        }

        GetComponent<AIController>().GenerateCommands();
        yield return StartCoroutine(GetComponent<AIController>().ExecuteCommands());

        ResetToSelectTile();
    }

    private void FixedUpdate()
    {
        if (GetComponent<AIController>().enemyUnits.Count == 0)
        {
            mode = ActionMode.Victory;

            foreach (GameObject unit in GetComponent<PlayerSession>().playerUnits)
            {
                PlayerPrefs.SetString(unit.GetComponent<Unit>().Name + "_name", unit.GetComponent<Unit>().Name);
                PlayerPrefs.SetString(unit.GetComponent<Unit>().Name + "_class", unit.GetComponent<Unit>().Class);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_level", unit.GetComponent<Unit>().Level);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_experience", unit.GetComponent<Unit>().Experience);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_health", unit.GetComponent<Unit>().Health);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_currentHealth", unit.GetComponent<Unit>().CurrentHealth);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_strength", unit.GetComponent<Unit>().Strength);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_magic", unit.GetComponent<Unit>().Magic);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_defense", unit.GetComponent<Unit>().Defense);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_resistance", unit.GetComponent<Unit>().Resistance);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_speed", unit.GetComponent<Unit>().Speed);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_luck", unit.GetComponent<Unit>().Luck);
                PlayerPrefs.SetInt(unit.GetComponent<Unit>().Name + "_skill", unit.GetComponent<Unit>().Skill);
                PlayerPrefsX.SetBool(unit.GetComponent<Unit>().Name + "_maySpawn", true);
                PlayerPrefs.Save();
            }
            StartCoroutine(NewLevel());

        }
        else if (GetComponent<PlayerSession>().playerUnits.Count == 0)
        {
            mode = ActionMode.Victory;

            StartCoroutine(GameOver());
        }
    }

    private IEnumerator NewLevel()
    {
        victoryFade.visible = true;
        yield return new WaitForSeconds(4.5f);
        blackness.visible = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("BasicScene");
    }

    private IEnumerator GameOver()
    {
        gameOverFade.visible = true;
        yield return new WaitForSeconds(4.5f);
        blackness.visible = true;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Intro Screen");
    }


    public void ResetToSelectTile()
    {
        switch (mode)
        {
            case ActionMode.MoveUnit:
                walkableTiles.DecolorTiles();
                unit.GetComponentInChildren<Outline>().enabled = false;
                unit = null;
                break;
            case ActionMode.SelectTile:
                break;
            case ActionMode.SelectWeapon:
                buttonsFade.visible = false;
                break;
            case ActionMode.SelectTarget:
                unit.GetComponentInChildren<Outline>().enabled = false;
                walkableTiles.DecolorTiles();
                predictionsFade.visible = false;
                break;
            case ActionMode.ConfirmBattle:
                predictionsFade.visible = false;
                break;
            default:
                break;
        }

        cursor.chooseAction.GetComponent<fadePanel>().visible = false;
        cursor.chooseAction.currentAction = CurrentAction.Move;
        unitStats.visible = false;
        mode = ActionMode.SelectTile;
    }
}

public enum ActionMode
{
    MoveUnit,
    SelectTile,
    SelectAction,
    SelectWeapon,
    SelectTarget,
    ConfirmBattle,
    WaitForBattle,
    ViewEnemyStats,
    EnemyTurn,
    Victory
}