using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;

using TriadCompiler.Parser.Common.Header;
using TriadCompiler.Parser.InfProcedure.Header;
using TriadCompiler.Parser.Common.Statement;

namespace TriadCompiler
    {
    /// <summary>
    /// ����� ��� ������� ��
    /// </summary>
    public partial class SimConditionParser : CommonParser
        {
        /// <summary>
        ///  ��������� ����
        /// </summary>
        private IConditionCodeBuilder codeBuilder
            {
            get
                {
                return Fabric.Instance.Builder as IConditionCodeBuilder;
                }
            }


        /// <summary>
        /// ������ ������
        /// </summary>
        /// <param name="endKey">��������� ���������� �������� ��������</param>
        public override void Compile( EndKeyList endKey )
            {
            codeBuilder.SetBaseClass( Builder.ICondition.BaseClass );

            GetNextKey();
            ModelCondition( endKey );
            }


        /// <summary>
        /// �������� ������� �������������
        /// </summary>
        /// <syntax>SimCondition HeaderName # IPHeader # { InfProcedure.Parse } Def StatementList EndIC</syntax>
        /// <param name="endKey">��������� ���������� �������� ��������</param>
        private void ModelCondition( EndKeyList endKey )
            {
            if ( currKey != Key.SimCondition )
                {
                err.Register( Err.Parser.WrongStartSymbol.ICondition, Key.SimCondition );
                SkipTo( endKey.UniteWith( Key.SimCondition ) );
                }
            if ( currKey == Key.SimCondition )
                {
                Accept( Key.SimCondition );

                //��� ������� �������������
                IConditionType icType = null;

                //��� ������� �������������
                HeaderName.Parse( endKey.UniteWith( InfHeader.StartKeys ).UniteWith( Key.IProcedure, Key.Define, Key.EndCond ),
                    delegate( string headerName )
                        {
                        icType = new IConditionType( headerName );
                        CommonArea.Instance.Register( icType );
                        } );

                varArea.AddArea();
                //������������ ����������� �������
                RegisterStandardFuntions();
                RegisterIProcedures(); //by jum
                //������������ � ������� ��������� ���������� � ��������� ��������
                CommonArea.Instance.Register( new VarType( TypeCode.Real, Builder.Routine.SystemTime ) );

                //������ //by jum
                CommonArea.Instance.Register(new DesignVarType( Builder.ICondition.CurrentModel, DesignTypeCode.Structure));

                this.designTypeName = icType.Name;
                codeBuilder.SetClassName( icType.Name );

                //���������
                InfHeader.Parse( endKey.UniteWith( Key.IProcedure, Key.Define, Key.EndCond ),
                    icType );

                //���������� �������������� ��������
                while ( currKey == Key.IProcedure )
                    {
                    //��������� ������� ������ � �������������
                    CommonParser currParser = Fabric.Instance.Parser;
                    CodeBuilder currBuilder = Fabric.Instance.Builder;

                    Fabric.Instance.Parser = new InfProcedureParser();
                    Fabric.Instance.Builder = new IProcedureCodeBuilder();

                    Fabric.Instance.Scanner.SaveSymbol( currSymbol );
                    Fabric.Instance.Parser.Compile( endKey.UniteWith( Key.IProcedure, Key.Define, Key.EndCond ) );
                    Fabric.Instance.Builder.Build();

                    Fabric.Instance.Scanner.SaveSymbol( Fabric.Instance.Parser.CurrentSymbol );
                    Fabric.Instance.Parser = currParser;
                    Fabric.Instance.Builder = currBuilder;
                    GetNextKey();
                    }

                Accept( Key.Define );

                codeBuilder.SetDoCheckMethod( StatementList.Parse( endKey.UniteWith( Key.EndCond ), StatementContext.Common ) );

                varArea.RemoveArea();

                //����� ������� �������������
                Accept( Key.EndCond );

                if ( !endKey.Contains( currKey ) )
                    {
                    err.Register( Err.Parser.WrongEndSymbol.ICondition, endKey.GetLastKeys() );
                    SkipTo( endKey );
                    }
                }
            }
        }
    }
